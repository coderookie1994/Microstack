using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using microstack.configuration;
using microstack.configuration.Models;
using microstack.Processor;
using Newtonsoft.Json;

namespace microstack.Commands.SubCommands
{
    [Command(Name = "run",
        Description="Run the stack of apps specified in the Microstack configuration json",
        ShowInHelpText = true,
        ExtendedHelpText = "Provide path to configuration using [options] or set MSTCK_JSON environment variable",
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
    public class Run : BaseCommand
    {
        private StackProcessor _spc;
        private IHostApplicationLifetime _lifetime;

        [Option(
            CommandOptionType.SingleValue,
            ValueName = "path",
            Description = "Path to microstack config file",
            LongName = "config-file",
            ShortName = "c",
            ShowInHelpText = true
        )]
        [FileExists]
        public string ConfigFile { get; set; }

        [Option(
            CommandOptionType.NoValue,
            ValueName="Verbose",
            ShortName="v",
            LongName = "verbose",
            Description = "Log output from processes",
            ShowInHelpText=true)]
        public bool Verbose { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            ValueName = "Microstack Profile",
            Description = "The profile to use from the microstack configuration file",
            LongName = "profile",
            ShortName = "p",
            ShowInHelpText = true
        )]
        public string Profile { get; set; }
        
        private Dictionary<string, List<Configuration>> _configurations;
        private ConfigurationProvider _configProvider;

        public Run(StackProcessor spc, 
            IHostApplicationLifetime lifetime,
            IConsole console,
            ConfigurationProvider configProvider)
        {
            _spc = spc;
            _lifetime = lifetime;
            _console = console;
            _configProvider = configProvider;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            var ct = _lifetime.ApplicationStopping;
            _spc.SetVerbosity(Verbose);

            try {
                _configProvider.SetContext(ConfigFile, Profile);
                var validationResult = _configProvider.Validate();
                if (validationResult.ReturnCode == 1)
                {
                    OutputError(validationResult.Message);
                    return 1;
                }
            } catch(Exception ex)
            {
                OnException(ex);
                return 1;
            }

            _configProvider.SetConfigurationContext();
            // Start stack
            try {
                await _spc.InitStack();
            } catch(Exception ex)
            {
                OnException(ex);
                return 1;
            }

            // Loop until CTRL+C is pressed
            while(!ct.IsCancellationRequested) { }

            _console.ResetColor();
            _console.Out.WriteLine("\r\nMicrostack stopping...\r\n");
            return 0;
        }
    }
}