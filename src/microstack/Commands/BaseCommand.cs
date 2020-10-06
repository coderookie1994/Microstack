using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using microstack.Helpers;

namespace microstack.Commands
{
    [HelpOption("--help")]
    public abstract class BaseCommand
    {
        protected ConsoleHelper _consoleHelper;
        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {
            return Task.FromResult(0);
        }

        protected void OnException(Exception ex)
        {
            OutputError(ex.Message);
        }

        protected void OuputToConsole(string data)
        {
            _consoleHelper.Print(data, ConsoleColor.White);
        }

        protected void OutputError(string message)
        {
            _consoleHelper.Print(message, ConsoleColor.Red);
        }
    }
}