using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
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
        private Timer _timer;
        private readonly ProcessSpawnManager _processSpawnManager;
        private readonly IConsole _console;
        private readonly IApplicationLifetime _lifetime;
        private IList<(string Name, Process Process)> _processTuples;

        public ProcessQueueTask(ProcessSpawnManager processSpawnManager,
            IConsole console,
            IApplicationLifetime lifetime)
        {
            this._processSpawnManager = processSpawnManager;
            _processTuples = new List<(string Name, Process Process)>();
            _console = console;
            this._lifetime = lifetime;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _timer = new Timer(SpawnQueuedProcess, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
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
                using(var pipe = new NamedPipeClientStream(".", "microstack_pipe", PipeDirection.InOut, PipeOptions.None))
                {
                    try{
                        pipe.Connect(5);
                        ProtoBuf.Serializer.Serialize(pipe, new ProcessContract() {ProcessId = process.Id, MicroStackPID = Process.GetCurrentProcess().Id});
                    }
                    catch (TimeoutException ex)
                    {
                        _console.ForegroundColor = ConsoleColor.DarkRed;
                        _console.Out.WriteLine($"Failed to register {processInfoObjectTuples.Name} with PID {process.Id} with Microstack Daemon, please ensure that Microstack Windows service is running by manually starting it or reinstalling Microstack");
                    }
                    finally
                    {
                        _console.Out.WriteLine($"Killing {processInfoObjectTuples.Name}");
                        process.Kill(true);
                        throw new Exception("Failed to connect to Microstack Daemon");
                    }
                }
            } catch(Exception ex)
            {
                _console.ForegroundColor = ConsoleColor.Red;
                _console.Out.WriteLine($"Failed to spawn {processInfoObjectTuples.Name}, {ex.Message}");
                _console.ResetColor();
                _lifetime.StopApplication();
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
                    _console.Out.WriteLine($"Killing {processTuple.Name}");
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