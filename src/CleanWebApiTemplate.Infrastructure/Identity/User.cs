using Microsoft.AspNetCore.Identity;

namespace CleanWebApiTemplate.Infrastructure.Identity;
public class User : IdentityUser<int>
{
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}