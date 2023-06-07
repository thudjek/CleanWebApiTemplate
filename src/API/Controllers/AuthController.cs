using API.Extensions;
using API.Settings;
using Application.Common.Interfaces;
using Application.Features.Auth.Commands.ConfirmEmail;
using Application.Features.Auth.Commands.ExternalLogin;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.GetExternalLoginTokens;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResendConfirmationEmail;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.RevokeRefreshToken;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
            return Ok();

        return Conflict(result.ToErrorModel());
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
            

        return BadRequest(result.ToErrorModel());
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
            return Ok(result.Value);        
        }

        return Unauthorized(result.ToErrorModel());
    }

    [HttpPost]
    [Route("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreshTokenCommand command)
    {
        if (await Mediator.Send(new RevokeRefreshTokenCommand()))
        {
            HttpContext.Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

        return BadRequest();
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ToErrorModel());
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
        var result = await Mediator.Send(command);

        if (result.IsSuccess)
            return Ok();

        return BadRequest(result.ToErrorModel());
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
    public IActionResult GoogleLogin([FromRoute] string provider)
    {
        var properties = new AuthenticationProperties() { RedirectUri = Url.Action("ExternalLoginCallback"), AllowRefresh = true };
        properties.Items["LoginProvider"] = provider;
        return Challenge(properties, provider);
    }

    [HttpGet]
    [Route("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var result = await Mediator.Send(new ExternalLoginCommand());

        if (!result.IsSuccess)
            return BadRequest(result.ToErrorModel());

        return Redirect($"{_webAppSettings.ExternalLoginReturnUrl}?email={result.Value.Email}&provider={result.Value.Provider}");
    }

    [HttpPost]
    [Route("external-login-tokens")]
    public async Task<IActionResult> ExternalLoginTokens([FromBody] GetExternalLoginTokensCommand command)
    {
        var tokensDto = await Mediator.Send(command);
        return Ok(tokensDto);
    }
}