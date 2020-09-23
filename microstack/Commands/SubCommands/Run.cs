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
    [Command(Name = "run", ShowInHelpText = true)]
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
        [Option("-c <path>")]
        [FileExists]
        public string ConfigFile { get; set; }

        public Run(StackProcessor spc, IHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            _spc = spc;
            _env = env;
            _lifetime = lifetime;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            var ct = _lifetime.ApplicationStopping;
            // ConfigFile = app.
            if (ConfigFile is null)
            {
                app.HelpOption();
                return 1;
            }
            if (!ConfigFile.EndsWith(".mstkc.json"))
                return 1;
            
            var configurations = JsonConvert.DeserializeObject<List<Configuration>>(File.ReadAllText(Path.Combine(ConfigFile)));

            _spc.InitStack(configurations);

            while(!ct.IsCancellationRequested) { }
            
            return 0;
        }
    }
}