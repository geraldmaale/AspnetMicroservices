using GreatIdeas.MailServices;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;

namespace Ordering.Infrastructure.Mail;

public class EmailService : IEmailService
{
    private readonly IMsGraphService _msGraphMailService;
    private readonly EmailSettings _emailSettings;

    public EmailService(IMsGraphService msGraphMailService, 
        IOptions<EmailSettings> emailSettings)
    {
        _msGraphMailService = msGraphMailService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<bool> UseMsGraph(Email email)
    {
        var mailResponse = await _msGraphMailService.SendEmailAsync(
            new EmailModel()
            {
                Body = email.Body,
                FromAddress = _emailSettings.FromAddress,
                FromName = _emailSettings.FromName,
                To = email.To,
                Subject = email.Subject
            });

        return mailResponse;
    }
}