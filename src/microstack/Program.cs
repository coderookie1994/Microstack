using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using microstack.Commands;
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
                })
                .RunCommandLineApplicationAsync<MicroStack>(args, cts.Token)
                .GetAwaiter()
                .GetResult();
        }
    }
}
