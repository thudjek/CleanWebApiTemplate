﻿using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Dtos.Auth;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _jwtSettings;
    public IdentityService(UserManager<User> userManager, IDateTimeService dateTimeService, SignInManager<User> signInManager, JwtSettings jwtSettings)
    {
        _userManager = userManager;
        _dateTimeService = dateTimeService;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings;
    }


    public async Task Test()
    {
        var accesTokenTime = _jwtSettings.AccessTokenValidityInMinutes;
        var refreshTokenTime = _jwtSettings.RefreshTokenValidityInDays;
        await Task.CompletedTask;
    }

    public async Task<Result> Register(string email, string password)
    {
        if (await _userManager.FindByEmailAsync(email) != null)
            return Result.Failure("User with that email already exists");

        var user = new User()
        {
            Email = email,
            UserName = email
        };

        var registerResult = await _userManager.CreateAsync(user, password);
        if (registerResult != IdentityResult.Success)
            throw new IdentityException(registerResult.Errors.Select(e => e.Description));

        return Result.Success();
    }

    public async Task<Result<TokensDto>> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return Result<TokensDto>.Failure("Wrong email or password");

        if (!user.EmailConfirmed)
            return Result<TokensDto>.Failure("Email is not confirmed");

        var claims = GetUserClaims(user);
        var tokensDto = await GetTokensForUser(user, claims);

        return Result<TokensDto>.Success(tokensDto);
    }

    public async Task<Result<TokensDto>> RefreshToken(string accessToken, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            return Result<TokensDto>.Failure();

        var principal = GetClaimsPrincipalFromAccessToken(accessToken);
        if (principal == null)
            return Result<TokensDto>.Failure("Access or refresh token is expired or invalid");

        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= _dateTimeService.Now)
            return Result<TokensDto>.Failure("Access or refresh token is expired or invalid");

        var tokenDto = await GetTokensForUser(user, principal.Claims.ToList());

        return Result<TokensDto>.Success(tokenDto);
    }

    public async Task<bool> RevokeRefreshToken(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
        
        return true;
    }

    public async Task<Result<string>> GetEmailConfirmationToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<string>.Failure("There was an error while processing the request");

        return Result<string>.Success(await _userManager.GenerateEmailConfirmationTokenAsync(user));
    }

    public async Task<Result<string>> GetPasswordResetToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result<string>.Failure("There was an error while processing the request");

        return Result<string>.Success(await _userManager.GeneratePasswordResetTokenAsync(user));
    }

    public async Task<Result<string>> GetChangeEmailToken(string currentEmail, string newEmail)
    {
        var user = await _userManager.FindByEmailAsync(currentEmail);
        if (user == null)
            return Result<string>.Failure("There was an error while processing the request");

        return Result<string>.Success(await _userManager.GenerateChangeEmailTokenAsync(user, newEmail));
    }

    public async Task<Result> ConfirmEmail(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure("There was an error while processing the request");

        var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);

        if (confirmEmailResult.Succeeded)
            return Result.Success();
        else
            return Result.Failure();
    }

    public async Task<Result> ResetPassword(string email, string token, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure("There was an error while processing the request");

        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);

        if (resetPasswordResult.Succeeded)
            return Result.Success();
        else
            return Result.Failure();
    }

    public async Task<Result<ExternalLoginInfoDto>> ExternalLogin()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (!info.Principal.Claims.Any())
            return Result<ExternalLoginInfoDto>.Failure("Error while trying to login");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        var claims = GetUserClaims(user);

        if (signInResult.Succeeded)
        {
            var tokenDto = await GetTokensForUser(user, claims);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "AccessToken", tokenDto.AccessToken);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "RefreshToken", tokenDto.RefreshToken);

            return Result<ExternalLoginInfoDto>.Success(new ExternalLoginInfoDto() { Email = user.Email, Provider = info.LoginProvider });
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (user == null)
            {
                user = new User()
                {
                    Email = email,
                    UserName = email
                };

                var createUserResult = await _userManager.CreateAsync(user);
                if (createUserResult != IdentityResult.Success)
                    throw new IdentityException(createUserResult.Errors.Select(e => e.Description));

            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (addLoginResult != IdentityResult.Success)
                throw new IdentityException(addLoginResult.Errors.Select(e => e.Description));

            await _signInManager.SignInAsync(user, false);

            var tokenDto = await GetTokensForUser(user, claims);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "AccessToken", tokenDto.AccessToken);
            await _userManager.SetAuthenticationTokenAsync(user, info.LoginProvider, "RefreshToken", tokenDto.RefreshToken);

            return Result<ExternalLoginInfoDto>.Success(new ExternalLoginInfoDto() { Email = user.Email, Provider = info.LoginProvider });
        }

        return Result<ExternalLoginInfoDto>.Failure("Error while trying to login");
    }

    public async Task<TokensDto> GetExternalLoginTokens(string email, string provider)
    {
        var tokenDto = new TokensDto();

        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(provider))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
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

        if (user != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        return claims;
    }

    private async Task<TokensDto> GetTokensForUser(User user, List<Claim> claims)
    {
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        _ = int.TryParse(_jwtSettings.RefreshTokenValidityInDays.ToString(), out int refreshTokenValidityInDays);

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
        _ = int.TryParse(_jwtSettings.AccessTokenValidityInMinutes.ToString(), out int tokenValidityInMinutes); //see if correctly parses int in options, same for refresh token

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            expires: _dateTimeService.Now.AddMinutes(_jwtSettings.AccessTokenValidityInMinutes), // see if that works, same for refresh token, maybe validate options
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
            return null;

        return principal;
    }
}