using AppName.API.Extensions;
using AppName.API.Settings;
using AppName.Application.Common;
using AppName.Application.Common.Interfaces;
using AppName.Application.Features.Auth.Commands.ConfirmEmail;
using AppName.Application.Features.Auth.Commands.ExternalLogin;
using AppName.Application.Features.Auth.Commands.ForgotPassword;
using AppName.Application.Features.Auth.Commands.GetExternalLoginTokens;
using AppName.Application.Features.Auth.Commands.Login;
using AppName.Application.Features.Auth.Commands.RefreshToken;
using AppName.Application.Features.Auth.Commands.Register;
using AppName.Application.Features.Auth.Commands.ResendConfirmationEmail;
using AppName.Application.Features.Auth.Commands.ResetPassword;
using AppName.Application.Features.Auth.Commands.RevokeRefreshToken;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AppName.API.Controllers;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ApiBaseController
{
    private readonly IDateTimeService _dateTimeService;
    private readonly WebAppSettings _webAppSettings;
    public AuthController(IDateTimeService dateTimeService, WebAppSettings webAppSettings)
    {
        _dateTimeService = dateTimeService;
        _webAppSettings = webAppSettings;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            HttpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, _dateTimeService.Now.AddYears(1));
            return Ok(new { result.Value.AccessToken });
        }
            

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        command.RefreshToken = HttpContext.GetValueFromCookie("refreshToken");
        HttpContext.Response.Cookies.Delete("refreshToken");
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            HttpContext.AddCookieToResponse("refreshToken", result.Value.RefreshToken, true, _dateTimeService.Now.AddYears(1));
            return Ok(new { result.Value.AccessToken });
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    [Route("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken()
    {
        HttpContext.Response.Cookies.Delete("refreshToken");
        await Mediator.Send(new RevokeRefreshTokenCommand());
        return NoContent();
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost]
    [Route("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    [Route("external-login/{provider}")]
    public IActionResult ExternalLogin([FromRoute] string provider)
    {
        var properties = new AuthenticationProperties() { RedirectUri = Url.Action("ExternalLoginCallback"), AllowRefresh = true };
        properties.Items["LoginProvider"] = provider;
        return Challenge(properties, provider);
    }

    [HttpGet]
    [Route("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var externalLoginInfoDto = await Mediator.Send(new ExternalLoginCommand());
        return Redirect($"{_webAppSettings.ExternalLoginReturnUrl}?email={externalLoginInfoDto.Email}&provider={externalLoginInfoDto.Provider}");
    }

    [HttpPost]
    [Route("external-login-tokens")]
    public async Task<IActionResult> ExternalLoginTokens([FromBody] GetExternalLoginTokensCommand command)
    {
        HttpContext.Response.Cookies.Delete("refreshToken");
        var tokensDto = await Mediator.Send(command);
        HttpContext.AddCookieToResponse("refreshToken", tokensDto.RefreshToken, true, _dateTimeService.Now.AddYears(1));
        return Ok(tokensDto);
    }
}