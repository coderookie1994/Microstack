using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using microstack.Abstractions;
using microstack.Commands;
using microstack.Extensions;
using microstack.Handlers;
using microstack.Processor;

namespace microstack
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => {
                    services.AddLogging();
                    services.AddSingleton<StackProcessor>();
                    services.RegisterHandlers(sp => {
                        sp.AddSingleton<StackHandler, GitHandler>();
                        sp.AddSingleton<StackHandler, ProcessHandler>();
                    });
                    services.AddTransient<HandlerExecutor>();
                })
                .RunCommandLineApplicationAsync<MicroStack>(args, cts.Token)
                .GetAwaiter()
                .GetResult();
        }
    }
}
