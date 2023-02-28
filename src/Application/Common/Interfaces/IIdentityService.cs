using Application.Dtos.Auth;

namespace Application.Common.Interfaces;
public interface IIdentityService
{
    Task<Result> Register(string email, string password);
    Task<Result<TokensDto>> Login(string email, string password);
    Task<Result<TokensDto>> RefreshToken(string accessToken, string refreshToken);
    Task<bool> RevokeRefreshToken(string email);
    Task<Result<string>> GetEmailConfirmationToken(string email);
    Task<Result<string>> GetPasswordResetToken(string email);
    Task<Result<string>> GetChangeEmailToken(string currentEmail, string newEmail);
    Task<Result> ConfirmEmail(string email, string token);
    Task<Result> ResetPassword(string email, string token, string password);
    Task<Result<ExternalLoginInfoDto>> ExternalLogin();
    Task<TokensDto> GetExternalLoginTokens(string email, string provider);
}
