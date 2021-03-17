using Microsoft.Extensions.Hosting;
using Microstack.CLI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Microstack.Tests.CommandLineTests
{
    public class ProgramTest
    {
        public void StartHost(string[] args, CancellationTokenSource cts)
        {
            Program.CreateHostBuilder(args)
                .RunCommandLineApplicationAsync<Microstack.CLI.Commands.MicroStack>(args, cts.Token);
        }

        public void StartDaemon()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Microstack.Daemon.WindowsService.Program.CreateHostBuilder(new string[] { }).Build().RunAsync();
            }
        }
    }
}
