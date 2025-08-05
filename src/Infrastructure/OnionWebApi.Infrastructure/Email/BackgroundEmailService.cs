namespace OnionWebApi.Infrastructure.Email;
public class BackgroundEmailService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Queue<EmailMessage> _emailQueue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    public BackgroundEmailService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task QueueEmailAsync(EmailMessage message)
    {
        await _semaphore.WaitAsync();
        try
        {
            _emailQueue.Enqueue(message);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessEmailQueue();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    private async Task ProcessEmailQueue()
    {
        await _semaphore.WaitAsync();
        try
        {
            while (_emailQueue.Count > 0)
            {
                var message = _emailQueue.Dequeue();

                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await emailService.SendEmailAsync(message);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
