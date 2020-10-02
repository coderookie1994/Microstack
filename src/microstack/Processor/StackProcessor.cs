using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using microstack.configuration;
using microstack.configuration.Models;
using microstack.Handlers;
using microstack.Models;

namespace microstack.Processor
{
    public class StackProcessor
    {
        private IConsole _console;
        private HandlerExecutor _executor;
        private ConfigurationProvider _configProvider;

        public bool IsInitialized { get; private set; }
        private bool _isVerbose { get; set; }
        public StackProcessor(IConsole console, 
            HandlerExecutor executor,
            ConfigurationProvider configProvider)
        {
            _console = console;
            _executor = executor;
            _configProvider = configProvider;
        }
        public void SetVerbosity(bool isVerbose) => _isVerbose = isVerbose;
        public async Task InitStack()
        {
            await _executor.Execute(_configProvider.Configurations, _isVerbose);
            await _console.Out.WriteLineAsync("Press CTRL+C to exit... \r\n");
        }
    }
}