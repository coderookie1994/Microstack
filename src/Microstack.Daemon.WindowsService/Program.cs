using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microstack.Daemon.WindowsService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UsePlatformSpecificHostService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<MicroStackListner>();
                services.AddSingleton<ProcessStateManager>();
            });

        public static IHostBuilder UsePlatformSpecificHostService(this IHostBuilder builder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return builder.UseWindowsService();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return builder.UseSystemd();

            return builder;
        }
    }
}
