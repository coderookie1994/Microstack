using System;
using System.Collections.Generic;
using System.IO;
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
        ExtendedHelpText = "Provide path to .mstkc.json using [options] or set MSTKC_JSON environment variable")]
    public class Run : BaseCommand
    {
        private StackProcessor _spc;
        private IHostEnvironment _env;
        private IHostApplicationLifetime _lifetime;

        // [Option(CommandOptionType.SingleValue, 
        //     ShortName = "c", 
        //     LongName = "configuration", 
        //     Description = "Service configuration", 
        //     ShowInHelpText = true)]
        [Option(
            CommandOptionType.SingleValue,
            ValueName = "path",
            Description = "Path to .mstkc.json",
            LongName = "configfile",
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
        public Run(StackProcessor spc, IHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            _spc = spc;
            _env = env;
            _lifetime = lifetime;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            var ct = _lifetime.ApplicationStopping;
            _spc.SetVerbosity(Verbose);

            List<Configuration> configurations;
            if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSTKC_JSON")))
            {
                ConfigFile = Path.Combine(Environment.GetEnvironmentVariable("MSTKC_JSON"));
                configurations = JsonConvert.DeserializeObject<List<Configuration>>(File.ReadAllText(Path.Combine(ConfigFile)));
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
            
            configurations = JsonConvert.DeserializeObject<List<Configuration>>(File.ReadAllText(Path.Combine(ConfigFile)));

            _spc.InitStack(configurations);

            while(!ct.IsCancellationRequested) { }

            return 0;
        }
    }
}