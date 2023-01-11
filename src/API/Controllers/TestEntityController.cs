using Application.Features.TestEntities.Commands;
using Application.Features.TestEntities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class TestEntityController : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var testEntitiesList = await Mediator.Send(new GetTestEntities());
        if (testEntitiesList == null)
            return Ok();

        return Ok(testEntitiesList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTestEntity([FromBody] AddNewTestEntityCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }
}
