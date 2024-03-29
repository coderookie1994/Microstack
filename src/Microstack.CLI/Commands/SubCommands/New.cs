using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microstack.CLI.Models;
using Microstack.CLI.Helpers;
using Newtonsoft.Json;

namespace Microstack.CLI.Commands.SubCommands
{
    [Command(
        Name = "new",
        Description = "Generate different kinds of microstack configuration files",
        ShowInHelpText = true,
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect
    )]
    public class New : BaseCommand
    {
        [Option(
            CommandOptionType.NoValue,
            Description = "Create a new .mstkc.json file",
            LongName = "config-file",
            ShortName = "c",
            ShowInHelpText = true
        )]
        public bool GenerateMstkcConfig { get; set; }

        public New(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
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
            var configuration = new Dictionary<string, List<Microstack.Configuration.Models.Configuration>>()
            {
                {
                    "sample_profile", new List<Microstack.Configuration.Models.Configuration>(){
                        new Microstack.Configuration.Models.Configuration(){
                            ProjectName = "<<PROJECT NAME>>",
                            NextProjectName = "<<PROJECT NAME OF THE DEPENDENT PROJECT>>",
                            Port = 0,
                            ConfigOverrides = new System.Collections.Generic.Dictionary<string, string>() {
                                {
                                    "KEY_TO_OVERRIDE", "VALUE_OF_OVERRIDDEN_KEY"
                                },
                            },
                            GitProjectRootPath = "<<PATH TO GIT ROOT>>",
                            GitBranchName = "<<REMOTE_NAME>>",
                            GitUrl = "<<GIT_URL>>",
                            PullLatest = false,
                            UseTempFs = false,
                            StartupProjectRelativePath = "<<PATH TO STARTUP PROJECT>>",
                        }
                    }
                }
            };    
            try {
                System.IO.File.WriteAllText(".mstkc_template.json", JsonConvert.SerializeObject(configuration, Formatting.Indented));
            } catch(Exception ex)
            {
                _consoleHelper.Print($"Failed to create .mstkc.json {ex.Message}");
            }
            
        }
    }
}