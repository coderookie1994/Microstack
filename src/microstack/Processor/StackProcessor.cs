using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using microstack.Models;

namespace microstack.Processor
{
    public class StackProcessor
    {
        private IList<Configuration> _configurations;
        private IList<ProcessStartInfo> _processInfoObjects;
        private IList<Process> _processes;
        private Dictionary<string, string> _processNames;
        private ILogger<StackProcessor> _logger;

        public bool IsInitialized { get; private set; }

        private bool _isVerbose { get; set; }
        public StackProcessor(ILogger<StackProcessor> logger)
        {
            _logger = logger;
        }

        public void SetVerbosity(bool isVerbose) => _isVerbose = isVerbose;
        public void InitStack(IList<Configuration> configurations)
        {
            _processNames = new Dictionary<string, string>();
            _configurations = configurations;
            if (IsInitialized)
                return;

            BuildProcessObjects();

            _processes = _processInfoObjects
                .Select(pInfo => 
                {
                    var process = Process.Start(pInfo);
                    // _processNames.Add(pInfo.)
                    process.ErrorDataReceived += new DataReceivedEventHandler(HandleError);
                    // process.WaitForExit();
                    // process.BeginErrorReadLine();
                    return process;
                })
                .ToList();
            IsInitialized = true;
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

        private void HandleError(object sendingProcess, DataReceivedEventArgs args)
        {

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