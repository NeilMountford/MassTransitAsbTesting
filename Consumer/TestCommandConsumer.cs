using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace Consumer
{
    public class TestCommandConsumer : IConsumer<ITestCommand>
    {
        public Task Consume(ConsumeContext<ITestCommand> context)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow}: Consuming Command");
            throw new System.NotImplementedException();
        }
    }
}