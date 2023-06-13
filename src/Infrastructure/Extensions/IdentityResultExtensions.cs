using Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Extensions;
public static class IdentityResultExtensions
{
    public static void ThrowIfNotSuccessul(this IdentityResult identityResult, string message = null)
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