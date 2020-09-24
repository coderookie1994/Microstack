using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using microstack.Commands.SubCommands;

namespace microstack.Commands
{
    [Command("microstack", 
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
        Description = "Seamlessly run your apps without opening Visual Studio")]
    [Subcommand(typeof(Run))]
    [Subcommand(typeof(New))]
    public class MicroStack : BaseCommand
    {
        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}