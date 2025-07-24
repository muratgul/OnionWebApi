using DotNetCore.CAP;

namespace UnionWebApi.Infrastructure.Messaging;
public class ConsumerService : ICapSubscribe
{
    [CapSubscribe("sample.rabbitmq.message")]
    public void Consumer(object mesaj)
    {
        Console.WriteLine("Servis : " + mesaj);
    }
}
