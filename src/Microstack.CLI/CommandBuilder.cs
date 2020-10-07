using System;
using System.Collections.Generic;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace Microstack.CLI
{
    public class CommandBuilder
    {
        private CommandLineApplication _app;
        public CommandBuilder()
        {
            _app = new CommandLineApplication();
        }

        public void ConfigureCommands()
        {
            _app.HelpOption(true);
        }
    }
}
