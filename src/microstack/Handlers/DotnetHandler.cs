using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using microstack.Abstractions;
using microstack.configuration.Models;

namespace microstack.Handlers
{
    public class DotnetHandler : StackHandler
    {
        private IList<microstack.configuration.Models.Configuration> _configurations;
        private IList<ProcessStartInfo> _processInfoObjects;
        private IList<Process> _processes = new List<Process>();
        private bool _isVerbose;
        private Dictionary<string, string> _processNames;
        private readonly IConsole _console;

        public DotnetHandler(IConsole console)
        {
            _console = console;
        }
        public async override Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            _isVerbose = isVerbose;
            _processNames = new Dictionary<string, string>();
            _configurations = configurations;

            _console.ForegroundColor = ConsoleColor.DarkYellow;
            _console.Out.WriteLine("Initializing apps ... \r\n"); 
            _console.ResetColor();

            BuildProcessObjects();

            for (var i = 0; i < _processInfoObjects.Count; i++)
            {
                var process = Process.Start(_processInfoObjects[i]);
                // process.ErrorDataReceived += new DataReceivedEventHandler(HandleError);
                _processes.Add(process);

                _console.ForegroundColor = ConsoleColor.Green;
                _console.Out.WriteLine($"Started {configurations[i].ProjectName} on https://localhost:{configurations[i].Port}");
                _console.ResetColor();
            }

            await base.Handle(configurations, isVerbose);
        }

        private void BuildProcessObjects()
        {
            _processInfoObjects = _configurations.Select(p => {

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

                return processStartInfo;
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
    }
}