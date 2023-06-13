using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeRefreshToken;
public class RevokeRefreshTokenCommand : IRequest
{
}

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    public RevokeRefreshTokenHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RevokeRefreshToken(_currentUserService.UserId);
    }
}