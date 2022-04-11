using GreatIdeas.MailServices;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;

namespace Ordering.Infrastructure.Mail;

public class EmailService : IEmailService
{
    private readonly ISendGridService _sendGridService;
    private readonly IMsGraphMailService _msGraphMailService;
    private readonly EmailSettings _emailSettings;

    public EmailService(ISendGridService sendGridService, 
        IMsGraphMailService msGraphMailService, 
        IOptions<EmailSettings> emailSettings)
    {
        _sendGridService = sendGridService;
        _msGraphMailService = msGraphMailService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> UseSendGrid(Email email)
    {
        var response = await _sendGridService.SendEmailAsync(_emailSettings.ApiKey!,
            new EmailModel() {
                Body = email.Body,
                FromAddress = _emailSettings.FromAddress,
                FromName = _emailSettings.FromName,
                To = email.To,
                Subject = email.Subject
            });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UseMsGraph(Email email)
    {
        var response = await _msGraphMailService.SendEmailAsync(
            new EmailModel()
            {
                Body = email.Body,
                FromAddress = _emailSettings.FromAddress,
                FromName = _emailSettings.FromName,
                To = email.To,
                Subject = email.Subject
            });

        return response;
    }
}