using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProtoBuf;

namespace microstack.Daemon.WindowsService
{
    public class MicroStackListner : BackgroundService
    {
        private readonly ProcessStateManager _processStateManager;
        public MicroStackListner(ProcessStateManager processStateManager)
        {
            _processStateManager = processStateManager;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            
            return base.StartAsync(cancellationToken);
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var p = Process.GetProcessesByName("microstack");
                using (var pipe = new NamedPipeServerStream("microstack_pipe", PipeDirection.InOut, 5))
                {
                    var managedThread = Thread.CurrentThread.ManagedThreadId;
                    try {
                        await pipe.WaitForConnectionAsync(stoppingToken);
                    } catch(Exception ex)
                    {
                        
                    }
                    Console.WriteLine("Connected");
                    var processContract = Serializer.Deserialize<ProcessContract>(pipe);
                    _processStateManager.AddProcess(processContract.ProcessId, processContract.MicroStackPID);
                    Console.WriteLine($"Registered {processContract.ProcessId} with MicroStack PID {processContract.MicroStackPID}");
                    pipe.Disconnect();
                }
            }
        }
    }
}