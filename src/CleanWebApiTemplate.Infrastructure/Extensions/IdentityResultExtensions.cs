using CleanWebApiTemplate.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace CleanWebApiTemplate.Infrastructure.Extensions;
public static class IdentityResultExtensions
{
    public static void ThrowIfNotSuccessful(this IdentityResult identityResult, string message = null)
    {
        if (!identityResult.Succeeded)
        {
            if (message is not null)
            {
                throw new IdentityException(message, identityResult.Errors.Select(e => e.Description));
            }

            throw new IdentityException(identityResult.Errors.Select(e => e.Description));
        }
    }
}