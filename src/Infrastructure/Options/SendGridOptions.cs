namespace Infrastructure.Options;
public class SendGridOptions
{
    private const string SectionName = "SendGrid";

    public string ApiKey { get; set; }
    public string From { get; set; }
    public string FromDisplayName { get; set; }
    public int EmailConfirmationTemplateId { get; set; }
    public int EmailConfirmationUrl { get; set; }
    public int PasswordResetTemplateId { get; set; }
    public int PasswordResetUrl { get; set; }
}