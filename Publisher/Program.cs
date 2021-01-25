using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace Publisher
{
    class Program
    {
        public static async Task Main()
        {
            var busController = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host(
                    "Endpoint=sb://mt-investigation.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4EGGfi7+peCtGdr1137a0+HBoUTf38nIVYjTpg28Ouo=");
                
                cfg.Message<ITestMessage>(x => x.SetEntityName("TestMessageTopic"));
                cfg.Message<ITestCommand>(x => x.SetEntityName("TestCommandTopic"));
                EndpointConvention.Map<ITestCommand>(new Uri("queue:TestApp-TestCommands"));
            });

            await busController.StartAsync();
            await busController.Publish<ITestMessage>(new TestMessage());
            await busController.Send<ITestCommand>(new TestCommand());
            await busController.StopAsync();
        }
    }
}