using Application.Common.Interfaces;
using Application.Dtos.TestEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TestEntities.Queries;
public class GetTestEntities : IQuery<List<TestEntityDto>>
{

}

public class GetTestEntitiesHandler : IRequestHandler<GetTestEntities, List<TestEntityDto>>
{
    private readonly IAppDbContext _dbContext;
    public GetTestEntitiesHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TestEntityDto>> Handle(GetTestEntities request, CancellationToken cancellationToken)
    {
        return await _dbContext.TestEntities
                    .Select(e => new TestEntityDto() 
                    {
                        TestProperty = e.TestProperty
                    })
                    .ToListAsync(cancellationToken: cancellationToken);
    }
}