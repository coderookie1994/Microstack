using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace microstack.Commands
{
    [HelpOption("--help")]
    public abstract class BaseCommand
    {
        protected IConsole _console;
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
            _console.ForegroundColor = System.ConsoleColor.White;
            _console.Out.Write(data);
            _console.ResetColor();
        }

        protected void OutputError(string message)
        {
            _console.ForegroundColor = System.ConsoleColor.Red;
            _console.Error.Write(message);
            _console.ResetColor();
        }
    }
}