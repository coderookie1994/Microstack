using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microstack.Daemon.WindowsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<MicroStackListner>();
                services.AddSingleton<ProcessStateManager>();
            });
    }
}
