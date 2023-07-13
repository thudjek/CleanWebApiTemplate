using AppName.Application.Common.Interfaces;
using MediatR;

namespace AppName.Application.Features.Auth.Commands.ForgotPassword;
public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public ForgotPasswordCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await _identityService.GetPasswordResetToken(request.Email);
        await _emailService.SendPasswordResetEmail(request.Email, token);
    }
}