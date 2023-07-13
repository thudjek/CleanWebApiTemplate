using AppName.Application.Common;
using AppName.Application.Common.Interfaces;
using AppName.Application.Dtos.Auth;
using MediatR;

namespace AppName.Application.Features.Auth.Commands.Login;
public class LoginCommand : IRequest<Result<TokensDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokensDto>>
{
    private readonly IIdentityService _identityService;
    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<TokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.Login(request.Email, request.Password);
    }
}
