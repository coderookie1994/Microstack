using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microstack.CLI.Abstractions;
using Microstack.CLI.BackgroundTasks;
using Microstack.CLI;
using Microstack.Configuration.Models;

namespace Microstack.CLI.Handlers
{
    public class DotnetHandler : StackHandler
    {
        private IList<Microstack.Configuration.Models.Configuration> _configurations;
        private IList<(string ProjectName, ProcessStartInfo ProcessObject)> _processInfoObjects;
        private IList<Process> _processes = new List<Process>();
        private bool _isVerbose;
        private Dictionary<string, string> _processNames;
        private readonly IConsole _console;
        public DotnetHandler(IConsole console,
            ProcessSpawnManager processSpawnManager,
            Microstack.Configuration.ConfigurationProvider configurationProvider) : base (processSpawnManager, configurationProvider)
        {
            _console = console;
            configurationProvider.OnConfigurationChange += ConfigurationChanged;
        }
        public override void PreHandle()
        {
            // var buildProcessObjects = configurationProvider.Configurations
            //     // .Where(c => c.UseTempFs.Equals(false))
            //     .Select(p => {
            //         var processStartInfo = new ProcessStartInfo();
            //         processStartInfo.UseShellExecute = false;
            //         processStartInfo.FileName = DotNetExe.FullPathOrDefault();
            //         processStartInfo.Arguments = $"build";
            //         processStartInfo.WorkingDirectory = Path.Combine(p.GitProjectRootPath, p.StartupProjectRelativePath);
            //         processStartInfo.RedirectStandardOutput = SetVerbosity(_isVerbose, p.Verbose);

            //         return (p.ProjectName, processStartInfo);
            // }).ToList();
            // foreach(var pObj in buildProcessObjects)
            // {
            //     buildProcs.Add(Process.Start(pObj.processStartInfo));
            // }
            base.PreHandle();
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
            ConsoleOutObjectConfigurations();

            await base.Handle(isVerbose);
        }

        public override void PostHandle()
        {
            processSpawnManager.QueueToSpawn(_processInfoObjects);
        }

        private void ConsoleOutObjectConfigurations()
        {
            for (var i = 0; i < _processInfoObjects.Count; i++)
            {
                _console.ForegroundColor = ConsoleColor.DarkYellow;
                _console.Out.WriteLine($"Starting {_configurations[i].ProjectName} on https://{_configurations[i].HostName ?? "localhost"}:{_configurations[i].Port}");

                _console.ForegroundColor = ConsoleColor.DarkYellow;
                _console.Out.WriteLine("Configurations overridden");
                _console.ForegroundColor = ConsoleColor.Green;

                foreach (var config in _configurations[i].ConfigOverrides)
                {
                    _console.Out.WriteLine($"\t {config.Key}: {config.Value}");
                }
                _console.ResetColor();
            }
        }

        private void BuildProcessObjects()
        {
            _processInfoObjects = _configurations
                // .Where(c => c.UseTempFs.Equals(false))
                .Select(p => {
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.UseShellExecute = false;
                    foreach(var confOverride in p.ConfigOverrides)
                    {
                        processStartInfo.Environment.Add(confOverride);
                    }
                    processStartInfo.FileName = DotNetExe.FullPathOrDefault();
                    // processStartInfo.Arguments = $"{p.StartupDllName ?? StartupDllName(Path.Combine(p.GitProjectRootPath, p.StartupProjectRelativePath))} --urls \"https://{p.HostName ?? "localhost"}:{p.Port}\"";
                    processStartInfo.Arguments = $"run --no-launch-profile --urls \"https://{p.HostName ?? "localhost"}:{p.Port}\"";
                    processStartInfo.WorkingDirectory = Path.Combine(p.GitProjectRootPath, p.StartupProjectRelativePath);
                    processStartInfo.RedirectStandardOutput = SetVerbosity(_isVerbose, p.Verbose);

                    return (p.ProjectName, processStartInfo);
            }).ToList();
        }

        private string StartupDllName(string startupProjectPath)
        {
            var dirInfo = new DirectoryInfo(startupProjectPath);
            return dirInfo.Name + ".dll";
        }

        private bool SetVerbosity(bool verboseConsole, bool verboseProcess)
        {
            if (verboseConsole)
                return false;
            if (verboseProcess)
                return false;
            return true;
        }

        private async void ConfigurationChanged(object sender, ConfigurationEventArgs e)
        {
            processSpawnManager.SigKill(_processInfoObjects.Select(p => p.ProjectName));
            _console.Out.WriteLine($"Restarting dotnet processes...");
            _configurations = e.UpdatedConfiguration;          
            BuildProcessObjects();
            ConsoleOutObjectConfigurations();
            PostHandle();
        }

        private string SetRunDirectory(string startupPath)
        {
            var dirInfo = new DirectoryInfo(Path.Combine(startupPath, "bin", "Debug"));
            return dirInfo.GetDirectories().FirstOrDefault().FullName;
        }
    }
}