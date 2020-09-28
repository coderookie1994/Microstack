using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ProtoBuf;

namespace microstack.Daemon.WindowsService
{
    public class MicroStackListner : BackgroundService
    {
        private BlockingCollection<int> _newProcesses;
        public MicroStackListner()
        {
            _newProcesses = new BlockingCollection<int>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            
            return base.StartAsync(cancellationToken);
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using (var pipe = new NamedPipeServerStream("microstack_pipe", PipeDirection.InOut, 5))
                {
                    var managedThread = Thread.CurrentThread.ManagedThreadId;
                    await pipe.WaitForConnectionAsync();
                    var processContract = Serializer.Deserialize<ProcessContract>(pipe);
                    _newProcesses.Add(processContract.ProcessId);
                }
            }
        }
    }
}