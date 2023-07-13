using AppName.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AppName.Infrastructure.Extensions;
public static class SignInResultExtensions
{
    public static void ThrowIfNotSuccessful(this SignInResult signInResult, string message = null)
    {
        if (!signInResult.Succeeded)
        {
            var errors = new List<string>();
            if (signInResult.IsNotAllowed)
            {
                errors.Add("Not allowed");
            }

            if (signInResult.IsLockedOut)
            {
                errors.Add("Locked out");
            }

            if (signInResult.RequiresTwoFactor)
            {
                errors.Add("Required two factor authentication");
            }

            if (message is not null)
            {
                throw new IdentityException(message, errors);
            }

            throw new IdentityException(errors);
            
        }
    }
}