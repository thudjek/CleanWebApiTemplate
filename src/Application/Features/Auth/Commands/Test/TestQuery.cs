using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Auth.Commands.Test;
public class TestQuery : IRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public TestQueryAddress Address { get; set; }
}

public class TestQueryAddress 
{ 
    public string Street { get; set; }
    public int Number { get; set; }
}

public class TestQueryHandler : IRequestHandler<TestQuery>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    public TestQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task Handle(TestQuery request, CancellationToken cancellationToken)
    {
        await _identityService.Test();
    }
}