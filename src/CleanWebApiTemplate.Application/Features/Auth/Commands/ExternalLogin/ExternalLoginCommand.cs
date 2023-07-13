using CleanWebApiTemplate.Application.Common.Interfaces;
using CleanWebApiTemplate.Application.Dtos.Auth;
using MediatR;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.ExternalLogin;
public class ExternalLoginCommand : IRequest<ExternalLoginInfoDto>
{
}

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ExternalLoginInfoDto>
{
    private readonly IIdentityService _identityService;
    public ExternalLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ExternalLoginInfoDto> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ExternalLogin();
    }
}