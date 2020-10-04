using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace microstack.Commands.SubCommands
{
    [Command(
        Name = "tempfs",
        Description = "Temporary directory used for setting up a branch of code without disturbing the existing workspace, can be used for quick debugging",
        ShowInHelpText = true,
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
    public class TempFs : BaseCommand
    {
        [Option(
            CommandOptionType.NoValue,
            ShortName = "s",
            LongName = "show"
        )]
        public bool Show { get; set; }

        public TempFs(IConsole console)
        {
            _console = console;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            if (!Show)
            {
                app.ShowHelp();
                return 1;
            }

            var microStackDir = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/Temp/MicroStack"));
            var microStackExists = Directory.Exists(microStackDir);
            if (!microStackExists)
            {
                _console.Out.WriteLine("No temporary workspaces found");
                return 0;
            }
            
            _console.ForegroundColor = ConsoleColor.DarkYellow;
            _console.Out.WriteLine("Found temporary workspaces");
            _console.ForegroundColor = ConsoleColor.DarkGreen;
            foreach(var dir in Directory.EnumerateDirectories(microStackDir))
            {
                _console.Out.WriteLine($"\t {dir}");
            }
            _console.ResetColor();
            return 0;
        }
    }
}