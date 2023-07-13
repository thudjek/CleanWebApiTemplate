using CleanWebApiTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.ConfirmEmail;
public class ConfirmEmailCommand : IRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly IIdentityService _identityService;
    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ConfirmEmail(request.Email, request.Token);
    }
}