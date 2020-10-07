using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microstack.CLI;
using Microstack.CLI.Models;
using Microstack.CLI.Handlers;
using Microstack.CLI.Helpers;
using ConfigurationProvider = Microstack.Configuration.ConfigurationProvider;
namespace Microstack.CLI.Processor
{
    public class StackProcessor
    {
        private ConsoleHelper _consoleHelper;
        private HandlerExecutor _executor;
        private ConfigurationProvider _configProvider;

        public bool IsInitialized { get; private set; }
        private bool _isVerbose { get; set; }
        public StackProcessor(ConsoleHelper consoleHelper, 
            HandlerExecutor executor,
            ConfigurationProvider configProvider)
        {
            _consoleHelper = consoleHelper;
            _executor = executor;
            _configProvider = configProvider;
        }
        public void SetVerbosity(bool isVerbose) => _isVerbose = isVerbose;
        public async Task InitStack()
        {
            await _executor.Execute(_isVerbose);
            _consoleHelper.Print("Press CTRL+C to exit... \r\n");
        }
    }
}