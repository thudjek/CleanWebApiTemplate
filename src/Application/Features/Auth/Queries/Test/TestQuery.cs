using Application.Common.Exceptions;
using MediatR;

namespace Application.Features.Auth.Queries.Test;
public class TestQuery : IRequest
{

}

public class TestQueryHandler : IRequestHandler<TestQuery>
{
    public TestQueryHandler()
    {
        
    }

    public Task Handle(TestQuery request, CancellationToken cancellationToken)
    {
        throw new IdentityException(new List<string> { "error1", "error2" });
    }
}