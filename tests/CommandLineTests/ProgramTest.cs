using Microsoft.Extensions.Hosting;
using Microstack.CLI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microstack.Tests.CommandLineTests
{
    public class ProgramTest
    {
        public Task StartHost(string[] args, CancellationTokenSource cts)
        {
            return Task.Run(() =>
            {
                cts.Token.ThrowIfCancellationRequested();
                Program.CreateHostBuilder(args)
                    .RunCommandLineApplicationAsync<Microstack.CLI.Commands.MicroStack>(args, cts.Token);
            }, cts.Token);
        }

        public Task StartDaemon()
        {
            return Microstack.Daemon.WindowsService.Program.CreateHostBuilder(new string[] { }).Build().RunAsync();
        }
    }
}
