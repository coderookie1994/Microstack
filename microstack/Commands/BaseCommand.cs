using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace microstack.Commands
{
    [HelpOption("--help")]
    public abstract class BaseCommand
    {
        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {
            return Task.FromResult(0);
        }
    }
}