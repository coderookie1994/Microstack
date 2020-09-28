using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;

namespace microstack.BackgroundTasks
{
    public class ProcessQueueTask : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ProcessSpawnManager _processSpawnManager;
        private readonly IConsole _console;
        private IList<(string Name, Process Process)> _processTuples;

        public ProcessQueueTask(ProcessSpawnManager processSpawnManager,
            IConsole console)
        {
            this._processSpawnManager = processSpawnManager;
            _processTuples = new List<(string Name, Process Process)>();
            _console = console;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
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
            } catch(Exception ex)
            {
                _console.ForegroundColor = ConsoleColor.Red;
                _console.Out.WriteLine($"Failed to spawn {processInfoObjectTuples.Name}, {ex.Message}");
                _console.ResetColor();
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