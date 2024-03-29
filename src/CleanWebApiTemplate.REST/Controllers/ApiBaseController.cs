﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanWebApiTemplate.API.Controllers;

[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    private ISender _mediator = null!;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
