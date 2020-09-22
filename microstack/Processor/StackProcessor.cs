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
        private ILogger<StackProcessor> _logger;

        public bool IsInitialized { get; private set; }

        public StackProcessor()
        {
            // _logger = logger;
        }
        public void InitStack(IList<Configuration> configurations)
        {
            _configurations = configurations;
            if (IsInitialized)
                return;

            BuildProcessObjects();

            _processes = _processInfoObjects
                .Select(pInfo => 
                {
                    var process = Process.Start(pInfo);
                    // _logger.LogInformation($"{pInfo.}")
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
                processStartInfo.Arguments = "run";
                processStartInfo.WorkingDirectory = Path.Combine(p.StartupProjectPath);
                
                return processStartInfo;
            }).ToList();
        }
    }
}