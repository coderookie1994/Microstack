using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using microstack.Abstractions;
using microstack.Commands;
using microstack.configuration;
using microstack.Extensions;
using microstack.git;
using microstack.git.Abstractions;
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
                    services.RegisterHandlers(sh => {
                        sh.AddHandler<GitHandler>();
                        sh.AddHandler<DotnetHandler>();
                    });
                    services.AddTransient<HandlerExecutor>();
                    services.AddSingleton<ConfigurationProvider>();
                    services.AddTransient<ICredentialProvider, GitCredentialProvider>();
                    services.AddTransient<GitOps>();
                })
                .RunCommandLineApplicationAsync<MicroStack>(args, cts.Token)
                .GetAwaiter()
                .GetResult();
        }
    }
}
