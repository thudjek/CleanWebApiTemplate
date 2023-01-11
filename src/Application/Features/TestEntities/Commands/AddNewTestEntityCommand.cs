using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.TestEntities.Commands;
public class AddNewTestEntityCommand : ICommand
{
    public string TestProperty { get; set; }
}

public class AddNewTestEntityCommandHandler : IRequestHandler<AddNewTestEntityCommand>
{
    private readonly IAppDbContext _dbContext;
    public AddNewTestEntityCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(AddNewTestEntityCommand request, CancellationToken cancellationToken)
    {
        var testEntity = new TestEntity()
        {
            TestProperty = request.TestProperty
        };

        _dbContext.TestEntities.Add(testEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}