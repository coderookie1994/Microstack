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

        public TempFs(IConsole console)
        {
            _console = console;
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
                _console.Out.WriteLine("No temporary workspaces found");
                return 0;
            }
            
            _console.ForegroundColor = ConsoleColor.DarkYellow;
            _console.Out.WriteLine("Found temporary workspaces");
            _console.ForegroundColor = ConsoleColor.DarkGreen;
            foreach(var dir in Directory.GetDirectories(microStackDir))
            {
                _console.Out.WriteLine($"\t {Path.GetFileName(dir)}");
            }
            _console.ResetColor();
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
                    } catch(IOException ex)
                    {
                        _console.ForegroundColor = ConsoleColor.DarkRed;
                        _console.Out.WriteLine("IOException encountered", ex);
                    }
                    catch(UnauthorizedAccessException ex)
                    {
                        _console.ForegroundColor = ConsoleColor.DarkRed;
                        _console.Out.WriteLine($"Access Denied, try opening prompt with elevated previlidges {ex.Message}");
                    }
                    finally
                    {
                        _console.ResetColor();
                    }
                }
                else
                {
                    _console.ForegroundColor = ConsoleColor.DarkRed;
                    _console.Out.WriteLine($"Specified workspace {Delete} not found");
                    _console.ResetColor();
                }
                return;
            }
            _console.ForegroundColor = ConsoleColor.DarkRed;
            _console.Out.WriteLine("No temporary workspaces found");
        }
    }
}