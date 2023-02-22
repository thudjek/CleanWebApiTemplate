using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword;
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
        var tokenResult = await _identityService.GetPasswordResetToken(request.Email);
        if (tokenResult.IsSuccess)
            await _emailService.SendPasswordResetEmail(request.Email, tokenResult.Value);
    }
}