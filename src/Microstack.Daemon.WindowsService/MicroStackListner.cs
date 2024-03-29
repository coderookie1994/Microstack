using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProtoBuf;

namespace Microstack.Daemon.WindowsService
{
    public class MicroStackListner : BackgroundService
    {
        private readonly ProcessStateManager _processStateManager;
        private object _spawnManagerLock = new object();
        public MicroStackListner(ProcessStateManager processStateManager)
        {
            _processStateManager = processStateManager;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var ps = new PipeSecurity();
            //var sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            //var par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
            //ps.AddAccessRule(par);
            while (!stoppingToken.IsCancellationRequested)
            {
                var p = Process.GetProcessesByName("microstack");
                using (var pipe = new NamedPipeServerStream("microstack_pipe", PipeDirection.InOut, 5,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    var managedThread = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine($"Managed thread {managedThread}");
                    try
                    {
                        await pipe.WaitForConnectionAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.WriteLine("Connected");
                    var processContract = Serializer.Deserialize<ProcessContract>(pipe);
                    lock (_spawnManagerLock)
                    {
                        _processStateManager.AddProcess(processContract.ProcessId, processContract.MicroStackPID);
                    }

                    Console.WriteLine(
                        $"Registered {processContract.ProcessId} with MicroStack PID {processContract.MicroStackPID}");
                    pipe.Disconnect();
                }
            }
        }
    }
}