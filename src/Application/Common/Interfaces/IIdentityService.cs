using Application.Dtos.Auth;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<int> Register(string email, string password);
    Task<Result<TokensDto>> Login(string email, string password);
    Task<Result<TokensDto>> RefreshToken(string accessToken, string refreshToken);
    Task RevokeRefreshToken(int userId);
    Task<string> GetEmailConfirmationToken(string email);
    Task<string> GetPasswordResetToken(string email);
    Task<string> GetChangeEmailToken(string currentEmail, string newEmail);
    Task ConfirmEmail(string email, string token);
    Task ResetPassword(string email, string token, string password);
    Task<ExternalLoginInfoDto> ExternalLogin();
    Task<TokensDto> GetExternalLoginTokens(string email, string provider);
}
