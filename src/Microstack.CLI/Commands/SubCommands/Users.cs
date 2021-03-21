using McMaster.Extensions.CommandLineUtils;
using Microstack.CLI.Helpers;
using Microstack.Configuration.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private readonly IUserSettingsProvider _userSettingsProvider;

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

        [Option(
            CommandOptionType.SingleValue,
            Description = "Add API Url",
            LongName = "url",
            ShortName = "ul",
            ShowInHelpText = true,
            ValueName = "URL"
            )]
        public string ApiUrl { get; set; }

        public Users(ConsoleHelper consoleHelper, IUserSettingsProvider userSettingsProvider)
        {
            _consoleHelper = consoleHelper;
            _userSettingsProvider = userSettingsProvider;
        }

        protected override async Task<int> OnExecute(CommandLineApplication app)
        {
            _app = app;
            //if (!Validate())
            //{
            //    return 1;
            //}

            if (!string.IsNullOrWhiteSpace(ApiUrl))
            {
                _userSettingsProvider.AddSetting(ApiUrl);
                return 0;
            }

            var settings = _userSettingsProvider.GetSettings();
            if (string.IsNullOrEmpty(settings))
            {
                OutputError("Missing api url, make sure to add a valid url, type microstack users --help for details");
                return 1;
            }

            if (!string.IsNullOrWhiteSpace(UserId))
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(settings);
                    try
                    {
                        var response = await (await client.GetAsync($"/api/users/{UserId}")).Content.ReadAsStringAsync();
                        var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(response), Formatting.Indented);
                        OuputToConsole(formattedJson);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        OutputError($"Error occurred while connecting to api {ex.Message}");
                        return 1;
                    }
                }
            }
            if (All)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(settings);
                    try
                    {
                        var response = await (await client.GetAsync($"/api/users")).Content.ReadAsStringAsync();
                        var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(response), Formatting.Indented);
                        OuputToConsole(formattedJson);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        OutputError($"Error occurred while connecting to api {ex.Message}");
                        return 1;
                    }
                }
            }
            
            app.ShowHelp();
            return 0;
        }

        private bool Validate()
        {
            var microStackUrl = Environment.GetEnvironmentVariable("MSTCK_API");
            var urlIsValid = Uri.TryCreate(microStackUrl, UriKind.Absolute, out Uri validUrl);
            if (string.IsNullOrWhiteSpace(microStackUrl) || !urlIsValid)
            {
                OutputError("MSTCK_API variable not found or empty or url is incorrect. Set a valid Microstack Url in order to use other users' workflows. Type microstack users --help for details");
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
