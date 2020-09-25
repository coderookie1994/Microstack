using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using microstack.Commands;
using microstack.Commands.SubCommands;
using microstack.Processor;
using Newtonsoft.Json;

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
