using System;
using McMaster.Extensions.CommandLineUtils;

namespace microstack.Helpers
{
    public class ConsoleHelper
    {
        private readonly IConsole _console;

        public ConsoleHelper(IConsole console)
        {
            _console = console;
        }

        public void Print(string message, ConsoleColor color = ConsoleColor.White)
        {
            _console.ForegroundColor = color;
            _console.Out.WriteLine(message);
            _console.ResetColor();
        }
    }
}