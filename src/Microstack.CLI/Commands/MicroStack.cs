using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microstack.CLI.Commands.SubCommands;

namespace Microstack.CLI.Commands
{
    [Command("microstack", 
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
        Description = "Seamlessly run your apps without opening Visual Studio")]
    [Subcommand(typeof(Run))]
    [Subcommand(typeof(New))]
    [Subcommand(typeof(TempFs))]
    public class MicroStack : BaseCommand
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public MicroStack(IHostApplicationLifetime hostApplicationLifetime)
        {
            this._hostApplicationLifetime = hostApplicationLifetime;
        }
        protected async override Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}