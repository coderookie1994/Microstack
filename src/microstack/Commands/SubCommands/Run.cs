using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using microstack.Models;
using microstack.Processor;
using Newtonsoft.Json;

namespace microstack.Commands.SubCommands
{
    [Command(Name = "run",
        Description="Run the stack of apps specified in .mstkc.json",
        ShowInHelpText = true,
        ExtendedHelpText = "Provide path to .mstkc.json using [options] or set MSTKC_JSON environment variable",
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

            Dictionary<string, List<Configuration>> configurations;
            
            try {
                DetermineEnvironmentPath();
            } catch (Exception ex)
            {
                OnException(ex);
                return 1;
            }

            if (ConfigFile is null)
            {
                app.ShowHelp();
                return 1;
            }

            if (!ConfigFile.EndsWith(".mstkc.json"))
            {
                app.ShowHelp();
                return 1;
            }
            
            try {
                configurations = ExtractConfigFromPath(ConfigFile);
            } catch(Exception ex)
            {
                OnException(ex);
                return 1;
            }
            if (configurations.Count > 1 && string.IsNullOrWhiteSpace(Profile))
            {
                OutputError("Multiple profiles found use -p to specify profile to use");
                return 1;
            }
            if (configurations.Count == 1)
                _spc.InitStack(configurations.First().Value);
            if (configurations.ContainsKey(Profile))
            {
                OuputToConsole($"Selected {Profile} \r\n");
                _spc.InitStack(configurations[Profile]);
            }
            else
            {
                OutputError($"{Profile} not found in configuration \r\n");
                return 0;
            }
            
            OuputToConsole("Apps initialized, Press CTRL+C to exit... \r\n"); 

            while(!ct.IsCancellationRequested) { }

            return 0;
        }

        private Dictionary<string, List<Configuration>> ExtractConfigFromPath(string path)
        {
            Dictionary<string, List<Configuration>> configurations = 
                new Dictionary<string, List<Configuration>>();
                
            try {
                configurations = JsonConvert.DeserializeObject<Dictionary<string, List<Configuration>>>(File.ReadAllText(Path.Combine(path)));
                foreach(var profile in configurations)
                {
                    var validationResult = profile.Value.Select(c => c.Validate());
                    if (validationResult.Any(v => v.IsValid == false))
                        throw new InvalidDataException();
                }
            } catch(Exception ex)
            {
                throw new InvalidDataException("Invalid configuration file format");
            }

            return configurations;
        }

        private void DetermineEnvironmentPath()
        {
            if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSTKC_JSON")))
            {
                ConfigFile = Path.Combine(Environment.GetEnvironmentVariable("MSTKC_JSON"));
                if (!File.Exists(ConfigFile))
                    throw new FileNotFoundException($"Config file not found at {ConfigFile}");
            }
        }
    }
}