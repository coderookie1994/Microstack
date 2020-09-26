using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using microstack.configuration.Models;
using microstack.Models;
using Newtonsoft.Json;

namespace microstack.Commands.SubCommands
{
    [Command(
        Name = "new",
        Description = "Generate different kinds of microstack configuration files",
        ShowInHelpText = true,
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect
    )]
    public class New : BaseCommand
    {
        private ILogger<New> _logger;

        [Option(
            CommandOptionType.NoValue,
            Description = "Create a new .mstkc.json file",
            LongName = "config-file",
            ShortName = "c",
            ShowInHelpText = true
        )]
        public bool GenerateMstkcConfig { get; set; }

        public New(ILogger<New> logger)
        {
            _logger = logger;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            if (!GenerateMstkcConfig)
            {
                app.ShowHelp();
                return 1;
            }
            
            GenerateMstkc();

            await base.OnExecute(app);
            return 0;
        }

        private void GenerateMstkc()
        {
            if (!GenerateMstkcConfig)
                return;
            
            // TEMPLATE CODE FOR OUTPUT
            var configuration = new Configuration(){
                ProjectName = "<<PROJECT NAME>>",
                NextProjectName = "<<PROJECT NAME OF THE DEPENDENT PROJECT>>",
                Port = 0,
                ConfigOverrides = new System.Collections.Generic.Dictionary<string, string>() {
                    {
                        "KEY_TO_OVERRIDE", "VALUE_OF_OVERRIDDEN_KEY"
                    },
                },
                GitProjectRootPath = "<<PATH TO GIT ROOT>>",
                StartupProjectPath = "<<PATH TO STARTUP PROJECT>>",
            };
            try {
                System.IO.File.WriteAllText(".mstkc.json", JsonConvert.SerializeObject(
                new List<Configuration>(){configuration}, Formatting.Indented));
            } catch(Exception ex)
            {
                _logger.LogInformation("Failed to create .mstkc.json", ex.Message);
            }
            
        }
    }
}