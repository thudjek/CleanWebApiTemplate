namespace CleanWebApiTemplate.Infrastructure.Settings;
public class SendGridSettings
{
    public const string SectionName = "SendGrid";

    public string ApiKey { get; set; }
    public string From { get; set; }
    public string FromDisplayName { get; set; }
    public string EmailConfirmationTemplateId { get; set; }
    public string EmailConfirmationUrl { get; set; }
    public string PasswordResetTemplateId { get; set; }
    public string PasswordResetUrl { get; set; }
}