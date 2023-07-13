using CleanWebApiTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.ResendConfirmationEmail;
public class ResendConfirmationEmailCommand : IRequest
{
    public string Email { get; set; }
}

public class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public ResendConfirmationEmailCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        var token = await _identityService.GetEmailConfirmationToken(request.Email);
        await _emailService.SendConfirmationEmail(request.Email, token);
    }
}