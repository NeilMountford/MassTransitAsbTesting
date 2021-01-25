using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace Consumer
{
    class Program
    {
        private static readonly TimeSpan[] RetryIntervals = new[] { 1, 1 }.Select(s => TimeSpan.FromSeconds(s)).ToArray();
        private static readonly TimeSpan[] DelayedRetryIntervals = new[] { 3, 6}.Select(s => TimeSpan.FromSeconds(s)).ToArray();
        private const string AsbConnectionString = "Endpoint=sb://mt-investigation.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=4EGGfi7+peCtGdr1137a0+HBoUTf38nIVYjTpg28Ouo=";

        
        public static async Task Main()
        {
            var services = new ServiceCollection();
            
            services.AddMassTransit(x =>
            {
                x.AddServiceBusMessageScheduler();
                x.AddConsumers(Assembly.GetEntryAssembly());
                x.UsingAzureServiceBus((context, cfg) => 
                {
                    cfg.Host(
                        AsbConnectionString);

                    cfg.UseServiceBusMessageScheduler();
                    // These don't seem to work at the top level
                    //cfg.UseScheduledRedelivery(r => r.Intervals(DelayedRetryIntervals));
                    //cfg.UseMessageRetry(r => r.Intervals(RetryIntervals));

                    cfg.Message<ITestMessage>(x => x.SetEntityName("TestMessageTopic"));
                    cfg.Message<ITestCommand>(x => x.SetEntityName("TestCommandTopic"));

                    var inputQueueName = "TestApp-TestEventInput";
                    var commandQueueName = "TestApp-TestCommands";

                    cfg.ReceiveEndpoint(inputQueueName.ToLowerInvariant(), e =>
                    {
                        e.UseScheduledRedelivery(r => r.Intervals(DelayedRetryIntervals));
                        e.UseMessageRetry(mr =>
                        {
                            mr.Intervals(RetryIntervals);
                        });
                        e.Consumer<TestConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(commandQueueName.ToLowerInvariant(), e =>
                    {
                        e.UseScheduledRedelivery(r => r.Intervals(DelayedRetryIntervals));
                        e.UseMessageRetry(mr =>
                        {
                            mr.Intervals(RetryIntervals);
                        });
                        e.Consumer<TestCommandConsumer>(context);
                    });
                });
            });
            
            var provider = services.BuildServiceProvider();
            var busControl = provider.GetRequiredService<IBusControl>();
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");
                await Task.Run(Console.ReadLine, source.Token);
                
            }
            finally
            {
                await busControl.StopAsync(source.Token);
            }
        }
    }
}