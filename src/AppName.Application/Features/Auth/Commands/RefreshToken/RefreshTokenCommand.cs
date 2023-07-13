using AppName.Application.Common;
using AppName.Application.Common.Interfaces;
using AppName.Application.Dtos.Auth;
using MediatR;

namespace AppName.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommand : IRequest<Result<TokensDto>>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokensDto>>
{
    private readonly IIdentityService _identityService;
    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<TokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return _identityService.RefreshToken(request.AccessToken, request.RefreshToken);
    }
}