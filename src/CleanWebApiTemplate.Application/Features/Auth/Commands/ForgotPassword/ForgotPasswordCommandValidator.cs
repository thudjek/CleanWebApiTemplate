﻿using FluentValidation;

namespace CleanWebApiTemplate.Application.Features.Auth.Commands.ForgotPassword;
public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");
    }
}