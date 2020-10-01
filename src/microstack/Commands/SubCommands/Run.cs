using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
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
        private IHostEnvironment _env;
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
        public Run(StackProcessor spc, 
            IHostEnvironment env, 
            IHostApplicationLifetime lifetime,
            IConsole console)
        {
            _spc = spc;
            _env = env;
            _lifetime = lifetime;
            _console = console;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            var ct = _lifetime.ApplicationStopping;
            _spc.SetVerbosity(Verbose);

            // Set source
            if (SetConfigFromSource(app) == 1)
                return 1;

            // Validate config
            if (ValidateConfig() == 1)
                return 1;
            
            if (ConfigFile != null)
            {
                // var fileSystemEvents = new FileSystemWatcher(ConfigFile);
                // fileSystemEvents.Changed += OnFileChanged;
            }

            // Start stack
            await StartStack();

            // Loop until CTRL+C is pressed
            while(!ct.IsCancellationRequested) { }

            _console.ResetColor();
            _console.Out.WriteLine("\r\nMicrostack stopping...\r\n");
            return 0;
        }

        private Dictionary<string, List<Configuration>> ExtractConfigFromPath()
        {       
            try {
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, List<Configuration>>>(File.ReadAllText(Path.Combine(ConfigFile)));
                foreach(var profile in _configurations)
                {
                    var validationResult = profile.Value.Select(c => c.Validate());
                    if (validationResult.Any(v => v.IsValid == false))
                        throw new InvalidDataException();
                }
            } catch(Exception ex)
            {
                throw new InvalidDataException("Invalid configuration file format");
            }

            return _configurations;
        }

        private void GetFromEnvironmentPath()
        {
            if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSTCK_JSON")))
            {
                ConfigFile = Path.Combine(Environment.GetEnvironmentVariable("MSTCK_JSON"));
                if (!File.Exists(ConfigFile))
                    throw new FileNotFoundException($"Config file not found at {ConfigFile}");
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, List<Configuration>>>(File.ReadAllText(Path.Combine(ConfigFile)));
            }
        }

        private int SetConfigFromSource(CommandLineApplication app)
        {
            try {
                // Path takes priority over env
                if (ConfigFile != null)
                    ExtractConfigFromPath();
                else
                    GetFromEnvironmentPath();
                if (_configurations == null || _configurations.Count == 0)
                {
                    app.ShowHelp();
                    return 1;
                }
            } catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }
            return 0;
        }

        private int ValidateConfig()
        {
            if (_configurations.Count > 1 && string.IsNullOrWhiteSpace(Profile))
            {
                OutputError("Multiple profiles found use -p to specify profile to use");
                return 1;
            }
            else if (_configurations.ContainsKey(Profile))
            {
                OuputToConsole($"Selected {Profile} \r\n");
            }
            else
            {
                OutputError($"{Profile} not found in configuration \r\n");
                return 1;
            }
            return 0;
        }

        private async Task<int> StartStack()
        {
            try {
                if (_configurations.Count == 1)
                    await _spc.InitStack(_configurations.First().Value);
                else if (Profile != null)
                    await _spc.InitStack(_configurations[Profile]);
            } catch(Exception ex)
            {
                OnException(ex);
                return 1;
            }

            return 0;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            
        }
    }
}