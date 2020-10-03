using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using microstack.BackgroundTasks.Models;

namespace microstack.BackgroundTasks
{
    public class ProcessQueueTask : IHostedService, IDisposable
    {
        private CancellationToken _cancellationToken;
        private Timer _spawnQueueScheduler;
        private Timer _killQueueScheduler;
        private readonly ProcessSpawnManager _processSpawnManager;
        private readonly IConsole _console;
        private readonly IHostApplicationLifetime _lifetime;
        private IList<(string Name, Process Process)> _processTuples;

        public ProcessQueueTask(ProcessSpawnManager processSpawnManager,
            IConsole console,
            IHostApplicationLifetime lifetime)
        {
            this._processSpawnManager = processSpawnManager;
            _processTuples = new List<(string Name, Process Process)>();
            _console = console;
            this._lifetime = lifetime;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _spawnQueueScheduler = new Timer(SpawnQueuedProcess, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            _killQueueScheduler = new Timer(KillRequestedProcesses, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        public void SpawnQueuedProcess(object state)
        {
            var processInfoObjectTuples = _processSpawnManager.Dequeue();
            
            if (processInfoObjectTuples.ProcessInfoObject == null)
                return;

            try 
            {
                var process = Process.Start(processInfoObjectTuples.ProcessInfoObject);
                _processTuples.Add((processInfoObjectTuples.Name, process));
                
                // Let users control registration if daemon fails
                bool.TryParse(Environment.GetEnvironmentVariable("DISABLE_MSTCK_PID_REG"), out var daemonRegistraction);

                if (!daemonRegistraction)
                {
                    using(var pipe = new NamedPipeClientStream(".", "microstack_pipe", PipeDirection.InOut, PipeOptions.None))
                    {
                        try{
                            pipe.Connect(5);
                            _console.Out.WriteLine($"Registering {processInfoObjectTuples.Name} PID {process.Id} with Microstack Daemon");
                            ProtoBuf.Serializer.Serialize(pipe, new ProcessContract() {ProcessId = process.Id, MicroStackPID = Process.GetCurrentProcess().Id});
                            _console.ResetColor();
                        }
                        catch (TimeoutException ex)
                        {
                            _console.ForegroundColor = ConsoleColor.DarkRed;
                            _console.Out.WriteLine($"Failed to register {processInfoObjectTuples.Name} with PID {process.Id} with Microstack Daemon, please ensure that Microstack Windows service is running by manually starting it or reinstalling Microstack");
                            _console.Out.WriteLine($"Killing {processInfoObjectTuples.Name}");
                            process.Kill(true);
                            throw new Exception("Failed to connect to Microstack Daemon");
                        }
                    }
                }
            } catch(Exception ex)
            {
                _console.ForegroundColor = ConsoleColor.DarkRed;
                _console.Out.WriteLine($"Failed to spawn {processInfoObjectTuples.Name}, {ex.Message}");
                _console.ResetColor();
                _lifetime.StopApplication();
            }
        }

        public void KillRequestedProcesses(object state)
        {
            var processToKill = _processSpawnManager.DequeueKillRequests();

            try {
                var pt = _processTuples.FirstOrDefault(t => t.Name.Equals(processToKill));
                if (pt.Name != null)
                {
                    pt.Process.Kill(true);
                    _processTuples.Remove(pt);
                }
            } catch(Exception ex) {

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var processTuple in _processTuples)
            {
                try {
                    processTuple.Process.Kill(true);
                } catch(Exception)
                {

                }
                finally{
                    _console.Out.WriteLine($"Stopping {processTuple.Name}");
                }
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _processTuples = null;
        }
    }
}