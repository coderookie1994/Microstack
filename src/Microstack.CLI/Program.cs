using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microstack.CLI.BackgroundTasks;
using Microstack.CLI.Commands;
using Microstack.CLI.Extensions;
using Microstack.Git;
using Microstack.Git.Abstractions;
using Microstack.CLI.Handlers;
using Microstack.CLI.Helpers;
using Microstack.CLI.Processor;
using Microstack.Configuration.Abstractions;
using Microstack.Configuration;

namespace Microstack.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            CreateHostBuilder(args)
                .RunCommandLineApplicationAsync<MicroStack>(args, cts.Token)
                .GetAwaiter()
                .GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<StackProcessor>();
                    services.RegisterHandlers(sh =>
                    {
                        sh.AddHandler<GitHandler>();
                        sh.AddHandler<DotnetHandler>();
                    });
                    services.AddTransient<HandlerExecutor>();
                    services.AddSingleton<Microstack.Configuration.ConfigurationProvider>();
                    services.AddSingleton<ICredentialProvider, GitCredentialProvider>();
                    services.AddTransient<IGitOps, GitOps>();
                    services.AddSingleton<ProcessQueueTask>();
                    services.AddHostedService(sp => sp.GetRequiredService<ProcessQueueTask>());
                    services.AddTransient<ConsoleHelper>();
                    services.AddTransient<IUserSettingsProvider, UserSettingsProvider>();
                });
    }
}
