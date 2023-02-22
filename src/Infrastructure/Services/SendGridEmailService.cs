using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;
public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly SendGridSettings _sendGridSettings;
    public SendGridEmailService(ISendGridClient sendGridClient, ILogger<SendGridEmailService> logger, IOptions<SendGridSettings> sendGridSettings)
    {
        _sendGridClient = sendGridClient;
        _logger = logger;
        _sendGridSettings = sendGridSettings.Value;
    }

    public async Task SendConfirmationEmail(string email, string token)
    {
        var url = _sendGridSettings.EmailConfirmationUrl.Replace("*email*", email).Replace("*token*", token);

        var sendGridMessage = new SendGridMessage();
        sendGridMessage.SetFrom(_sendGridSettings.From, _sendGridSettings.FromDisplayName);
        sendGridMessage.AddTo(email);
        sendGridMessage.SetSubject("Email Confirmation");
        sendGridMessage.SetTemplateId(_sendGridSettings.EmailConfirmationTemplateId);
        sendGridMessage.SetTemplateData(new { confirmationUrl = url });

        var emailResponse = await _sendGridClient.SendEmailAsync(sendGridMessage);

        if (!emailResponse.IsSuccessStatusCode)
        {
            var message = await emailResponse.Body.ReadAsStringAsync();
            _logger.LogError("Error while trying to send confirmation email. Message: {message}", message);
        }
    }

    public async Task SendPasswordResetEmail(string email, string token)
    {
        var url = _sendGridSettings.PasswordResetUrl.Replace("*email*", email).Replace("*token*", token);

        var sendGridMessage = new SendGridMessage();
        sendGridMessage.SetFrom(_sendGridSettings.From, _sendGridSettings.FromDisplayName);
        sendGridMessage.AddTo(email);
        sendGridMessage.SetSubject("Password Reset");
        sendGridMessage.SetTemplateId(_sendGridSettings.PasswordResetTemplateId);
        sendGridMessage.SetTemplateData(new { passwordResetUrl = url });

        var emailResponse = await _sendGridClient.SendEmailAsync(sendGridMessage);

        if (!emailResponse.IsSuccessStatusCode)
        {
            var message = await emailResponse.Body.ReadAsStringAsync();
            _logger.LogError("Error while trying to send password reset email. Message: {message}", message);
        }
    }
}