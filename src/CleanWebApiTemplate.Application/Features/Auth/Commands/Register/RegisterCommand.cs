﻿using CleanWebApiTemplate.Application.Common;
using CleanWebApiTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public RegisterCommandHandler(IIdentityService identityService, IEmailService emailService)
    {
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.Register(request.Email, request.Password);

        if (userId != 0)
        {
            var token = await _identityService.GetEmailConfirmationToken(request.Email);
            await _emailService.SendConfirmationEmail(request.Email, token);
        }

        return Result.Success();
    }
}