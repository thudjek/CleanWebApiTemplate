using CleanWebApiTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.ResetPassword;
public class ResetPasswordCommand : IRequest
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IIdentityService _identityService;
    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ResetPassword(request.Email, request.Token, request.Password);
    }
}