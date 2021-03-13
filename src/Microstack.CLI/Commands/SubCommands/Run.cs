using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microstack.CLI;
using Microstack.CLI.Models;
using Microstack.Git.Abstractions;
using Microstack.CLI.Helpers;
using Microstack.CLI.Processor;
using Newtonsoft.Json;

namespace Microstack.CLI.Commands.SubCommands
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
        private string Username { get; set; }
        private string Password { get; set; }
        private string Email { get; set; }
        
        private Dictionary<string, List<Microstack.Configuration.Models.Configuration>> _configurations;
        private Microstack.Configuration.ConfigurationProvider _configProvider;
        private ICredentialProvider _credentialProvider;
        public Run(StackProcessor spc, 
            IHostApplicationLifetime lifetime,
            ConsoleHelper consoleHelper,
            Microstack.Configuration.ConfigurationProvider configProvider,
            ICredentialProvider credProvider)
        {
            _spc = spc;
            _lifetime = lifetime;
            _configProvider = configProvider;
            _credentialProvider = credProvider;
            _consoleHelper = consoleHelper;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            var ct = _lifetime.ApplicationStopping;
            _spc.SetVerbosity(Verbose);

            try {
                _configProvider.SetContext(ConfigFile, Profile);
                if (!_configProvider.IsContextSet)
                {
                    app.ShowHelp();
                    return 1;
                }
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

            if (_configProvider.Configurations.Any(c => c.UseTempFs == true))
            {
                _consoleHelper.Print("TempFS is set to true, enter git credentials");
                Username = Prompt.GetString("Enter git Username");
                Password = Prompt.GetPassword("Enter git token");
                Email = Prompt.GetString("Enter git email");
                _credentialProvider.SetCredentials(Username, Password, Email);
            }

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

            _consoleHelper.Print("\r\nMicrostack stopping...\r\n");
            return 0;
        }
    }
}