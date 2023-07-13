using AppName.Application.Common;
using AppName.Application.Common.Exceptions;
using AppName.Application.Common.Interfaces;
using AppName.Application.Dtos.Auth;
using AppName.Infrastructure.Extensions;
using AppName.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppName.Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<IdentityService> _logger;
    public IdentityService(UserManager<User> userManager, IDateTimeService dateTimeService, SignInManager<User> signInManager, JwtSettings jwtSettings, ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _dateTimeService = dateTimeService;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public async Task<int> Register(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            _logger.LogWarning("Register failed: email address already in use by user {UserId}", user.Id);
        }
        else
        {
            user = new User()
            {
                Email = email,
                UserName = email
            };

            var registerResult = await _userManager.CreateAsync(user, password);
            registerResult.ThrowIfNotSuccessul("Error during registration");
        }

        return user.Id;
    }

    public async Task<Result<TokensDto>> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Result<TokensDto>.Fail("Wrong email or password");
        }

        if (!user.EmailConfirmed)
        {
            return Result<TokensDto>.Fail("Email is not confirmed");
        }

        var claims = GetUserClaims(user);
        var tokensDto = await GetTokensForUser(user, claims);

        return Result<TokensDto>.Success(tokensDto);
    }

    public async Task<Result<TokensDto>> RefreshToken(string accessToken, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<TokensDto>.Fail("Access or refresh token is expired or invalid");
        }

        var principal = GetClaimsPrincipalFromAccessToken(accessToken);
        if (principal is null)
        {
            return Result<TokensDto>.Fail("Access or refresh token is expired or invalid");
        }

        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= _dateTimeService.Now)
        {
            return Result<TokensDto>.Fail("Access or refresh token is expired or invalid");
        }

        var tokensDto = await GetTokensForUser(user, principal.Claims.ToList());

        return Result<TokensDto>.Success(tokensDto);
    }

    public async Task RevokeRefreshToken(int userId)
    {
        if (userId != 0)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                _logger.LogWarning("Revoking refresh token failed: User {UserId} is null", userId);
                return;
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }

    public async Task<string> GetEmailConfirmationToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new IdentityException("Cannot get email confirmation token: User is null");
        }

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<string> GetPasswordResetToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new IdentityException("Cannot get password reset token: User is null");
        }

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<string> GetChangeEmailToken(string currentEmail, string newEmail)
    {
        var user = await _userManager.FindByEmailAsync(currentEmail);
        if (user is null)
        {
            throw new IdentityException("Cannot get change email token: User is null");
        }

        return await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
    }

    public async Task ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new IdentityException("Cannot confirm email: User is null");
        }

        var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);
        confirmEmailResult.ThrowIfNotSuccessul("Error during email confirmation");
    }

    public async Task ResetPassword(string email, string token, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new IdentityException("Cannot reset password: User is null");
        }

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);
        resetPasswordResult.ThrowIfNotSuccessul("Error during password reset");
    }

    public async Task<ExternalLoginInfoDto> ExternalLogin()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (!info.Principal.Claims.Any())
        {
            throw new IdentityException("Error while trying to get external login info: no claims in principal");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        var claims = GetUserClaims(user);

        if (signInResult.Succeeded)
        {
            await SetTokensForExternalLogin(user, claims, info.LoginProvider);
            return new ExternalLoginInfoDto() { Email = user.Email, Provider = info.LoginProvider };
        }

        if (user is null)
        {
            user = new User()
            {
                Email = email,
                UserName = email
            };

            var createUserResult = await _userManager.CreateAsync(user);
            createUserResult.ThrowIfNotSuccessul("Error while trying to create user for external login");
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        addLoginResult.ThrowIfNotSuccessul($"Error while trying to add external login for a user {user.Id}");

        await _signInManager.SignInAsync(user, false);

        await SetTokensForExternalLogin(user, claims, info.LoginProvider);
        return new ExternalLoginInfoDto() { Email = user.Email, Provider = info.LoginProvider };
    }

    public async Task<TokensDto> GetExternalLoginTokens(string email, string provider)
    {
        var tokenDto = new TokensDto();

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(provider))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                tokenDto.AccessToken = await _userManager.GetAuthenticationTokenAsync(user, provider, "AccessToken");
                tokenDto.RefreshToken = await _userManager.GetAuthenticationTokenAsync(user, provider, "RefreshToken");

                await _userManager.RemoveAuthenticationTokenAsync(user, provider, "AccessToken");
                await _userManager.RemoveAuthenticationTokenAsync(user, provider, "RefreshToken");
            }
        }

        return tokenDto;
    }

    private static List<Claim> GetUserClaims(User user)
    {
        var claims = new List<Claim>();

        if (user is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        return claims;
    }

    private async Task SetTokensForExternalLogin(User user, List<Claim> claims, string loginProvider)
    {
        var tokenDto = await GetTokensForUser(user, claims);
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider, "AccessToken", tokenDto.AccessToken);
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider, "RefreshToken", tokenDto.RefreshToken);
    }

    private async Task<TokensDto> GetTokensForUser(User user, List<Claim> claims)
    {
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _dateTimeService.Now.AddDays(_jwtSettings.RefreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);

        return new TokensDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateAccessToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            expires: _dateTimeService.Now.AddMinutes(_jwtSettings.AccessTokenValidityInMinutes),
            claims: claims,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetClaimsPrincipalFromAccessToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            return null;
        }

        return principal;
    }
}