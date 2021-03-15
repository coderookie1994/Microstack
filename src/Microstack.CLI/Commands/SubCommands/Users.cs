﻿using McMaster.Extensions.CommandLineUtils;
using Microstack.CLI.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.CLI.Commands.SubCommands
{
    [Command(Name = "users",
        Description = "Share or download workflows built by other users",
        ShowInHelpText = true,
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
    public class Users : BaseCommand
    {
        private CommandLineApplication _app;

        [Option(
            CommandOptionType.NoValue,
            Description = "List all the users",
            LongName = "all-users",
            ShortName = "all",
            ShowInHelpText = true)]
        public bool All { get; set; }

        [Option(
            CommandOptionType.SingleValue,
            Description = "List workflows for a user",
            LongName = "user",
            ShortName = "u",
            ShowInHelpText = true,
            ValueName = "UserId")]
        public string UserId { get; set; }

        public Users(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            _app = app;
            if (!Validate())
            {
                return 1;
            }    


            return 0;
        }

        private bool Validate()
        {
            var microStackUrl = Environment.GetEnvironmentVariable("MSTCK_API");
            var urlIsValid = Uri.TryCreate(microStackUrl, UriKind.Absolute, out Uri validUrl);
            if (string.IsNullOrWhiteSpace(microStackUrl) || !urlIsValid)
            {
                OutputError("MSTCK_API variable not found or empty or url is incorrect. Set a valid Microstack Url in order to use other users' workflows.");
                return false;
            }

            if (!All)
            {
                _app.ShowHelp();
                return false;
            }

            return true;
        }
    }
}