namespace AppName.Infrastructure.Settings;
public class JwtSettings
{
    public const string SectionName = "JWT";

    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
    public string Secret { get; set; }
    public int AccessTokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}