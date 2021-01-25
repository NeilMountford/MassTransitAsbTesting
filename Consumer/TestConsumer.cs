using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace Consumer
{
    public class TestConsumer : IConsumer<ITestMessage>
    {
        public Task Consume(ConsumeContext<ITestMessage> context)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow}: Consuming Event");
            throw new System.NotImplementedException();
        }
    }
}