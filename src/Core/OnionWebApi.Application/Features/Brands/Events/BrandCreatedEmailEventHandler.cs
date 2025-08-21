namespace OnionWebApi.Application.Features.Brands.Events;
public class BrandCreatedEmailEventHandler : INotificationHandler<BrandCreateDomainEvent>
{
    private readonly IEmailService _emailService;
    public BrandCreatedEmailEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(BrandCreateDomainEvent notification, CancellationToken cancellationToken)
    {
        var emailMessage = new EmailMessage
        {
            To = "to@mail.com",
            Subject = "New Brand Created",
            Body = $"A new brand has been created with the name: {notification.Name}",
            IsHtml = true,            
        };

        await _emailService.SendEmailAsync(emailMessage);
    }
}
