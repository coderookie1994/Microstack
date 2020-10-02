using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnyDiff.Extensions;
using McMaster.Extensions.CommandLineUtils;
using microstack.Abstractions;
using microstack.BackgroundTasks;
using microstack.configuration;
using microstack.configuration.Models;

namespace microstack.Handlers
{
    public class DotnetHandler : StackHandler
    {
        private IList<microstack.configuration.Models.Configuration> _configurations;
        private IList<(string ProjectName, ProcessStartInfo ProcessObject)> _processInfoObjects;
        private IList<Process> _processes = new List<Process>();
        private bool _isVerbose;
        private Dictionary<string, string> _processNames;
        private readonly IConsole _console;
        protected new bool raiseEventOnHandleComplete = true;
        public DotnetHandler(IConsole console,
            ProcessSpawnManager processSpawnManager,
            ConfigurationProvider configurationProvider) : base (processSpawnManager, configurationProvider)
        {
            _console = console;
            configurationProvider.OnConfigurationChange += ConfigurationChanged;
        }
        public async override Task Handle(bool isVerbose)
        {
            _isVerbose = isVerbose;
            _processNames = new Dictionary<string, string>();
            _configurations = configurationProvider.Configurations;

            _console.ForegroundColor = ConsoleColor.DarkYellow;
            _console.Out.WriteLine("Initializing apps ... \r\n"); 
            _console.ResetColor();

            BuildProcessObjects();

            for (var i = 0; i < _processInfoObjects.Count; i++)
            {
                _console.ForegroundColor = ConsoleColor.DarkYellow;
                _console.Out.WriteLine($"Starting {_configurations[i].ProjectName} on https://{_configurations[i].HostName ?? "localhost"}:{_configurations[i].Port}");
                
                _console.ForegroundColor = ConsoleColor.DarkYellow;
                _console.Out.WriteLine("Configurations overridden");
                _console.ForegroundColor = ConsoleColor.Green;

                foreach(var config in _configurations[i].ConfigOverrides)
                {
                    _console.Out.WriteLine($"\t {config.Key}: {config.Value}");
                }
                _console.ResetColor();
            }

            await base.Handle(isVerbose);
        }

        public override void OnHandleComplete()
        {
            processSpawnManager.QueueToSpawn(_processInfoObjects);
        }

        private void BuildProcessObjects()
        {
            _processInfoObjects = _configurations
                // .Where(c => c.ProcessType.Equals(ProcessTypes.Dotnet))
                .Select(p => {
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.UseShellExecute = false;
                    foreach(var confOverride in p.ConfigOverrides)
                    {
                        processStartInfo.Environment.Add(confOverride);
                    }
                    processStartInfo.FileName = DotNetExe.FullPathOrDefault();
                    processStartInfo.Arguments = $"run --no-launch-profile --urls \"https://localhost:{p.Port}\"";
                    processStartInfo.WorkingDirectory = Path.Combine(p.StartupProjectPath);
                    processStartInfo.RedirectStandardOutput = SetVerbosity(_isVerbose, p.Verbose);

                    return (p.ProjectName, processStartInfo);
            }).ToList();
        }

        private bool SetVerbosity(bool verboseConsole, bool verboseProcess)
        {
            if (verboseConsole)
                return false;
            if (verboseProcess)
                return false;
            return true;
        }

        private void ConfigurationChanged(object sender, ConfigurationEventArgs e)
        {
            // var changeInProjects = new HashSet<string>();

            // var diff = _configurations.Diff(e.UpdatedConfiguration);
            // foreach(var difference in diff)
            // {
            //     if (difference.Delta != null)
            //     {
            //         if (difference.Property)
            //     }
            // }
        }
    }
}