using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microstack.CLI.Helpers;

namespace Microstack.CLI.Commands.SubCommands
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
            LongName = "show",
            Description = "Show available workspaces in the temporary directory"
        )]
        public bool Show { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            ShortName = "d",
            LongName = "delete",
            Description = "Delete the temporary workspace",
            ValueName = "WorkspaceName",
            ShowInHelpText = true
        )]
        public string Delete { get; set; }

        public TempFs(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            if(!string.IsNullOrWhiteSpace(Delete))
            {
                DeleteWorkspace();
                return 0;
            }

            if (!Show)
            {
                app.ShowHelp();
                return 1;
            }

            var microStackDir = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/Temp/MicroStack"));
            var microStackExists = Directory.Exists(microStackDir);
            if (!microStackExists)
            {
                _consoleHelper.Print("No temporary workspaces found");
                return 0;
            }
            
            _consoleHelper.Print("Found temporary workspaces", ConsoleColor.DarkYellow);
            foreach(var dir in Directory.GetDirectories(microStackDir))
            {
                _consoleHelper.Print($"\t {Path.GetFileName(dir)}", ConsoleColor.DarkGreen);
            }
            return 0;
        }

        private void DeleteWorkspace()
        {
            var microStackDir = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%/AppData/Local/Temp/MicroStack"));
            if (Directory.Exists(microStackDir))
            {
                var specifiedDir = Path.Combine(microStackDir, Delete);
                if (Directory.Exists(specifiedDir))
                {
                    try {
                        Directory.Delete(specifiedDir, true);
                    } 
                    catch(IOException ex)
                    {
                        _consoleHelper.Print("IOException encountered", ConsoleColor.DarkRed);
                    }
                    catch(UnauthorizedAccessException ex)
                    {
                        _consoleHelper.Print($"Access Denied, try opening prompt with elevated previlidges {ex.Message}", ConsoleColor.DarkRed);
                    }
                }
                else
                {
                    _consoleHelper.Print($"Specified workspace {Delete} not found", ConsoleColor.DarkRed);
                }
                return;
            }
            _consoleHelper.Print("No temporary workspaces found", ConsoleColor.DarkRed);
        }
    }
}