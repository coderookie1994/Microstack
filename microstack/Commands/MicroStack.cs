using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using microstack.Commands.SubCommands;

namespace microstack.Commands
{
    [Command("microstack")]
    [Subcommand(typeof(Run))]
    public class MicroStack : BaseCommand
    {
        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            // app.Execute();
            return 0;
        }
    }
}