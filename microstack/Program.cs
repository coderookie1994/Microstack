using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using microstack.Models;
using Newtonsoft.Json;

namespace microstack
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Description = "Run your applications without using Visual Studio or manually running dotnet run",
                Name = "microstack"
            };

            app.HelpOption(true);
            app.Command("config", cmd =>
            {
                cmd.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect;

                var configNameArg = cmd.Option("-n|--name <NAME>", "Configuration name for set of apps, name as .ms_<NAME>.json", CommandOptionType.SingleValue).IsRequired();

                cmd.OnExecute(() =>
                {
                    var configName = configNameArg.Value();

                    if (string.IsNullOrEmpty(configName))
                        cmd.ShowHelp();

                    var directory = Directory.GetCurrentDirectory();

                    var filePath = Path.Combine(directory, $".ms_{configName}.json");
                    if (!File.Exists(filePath))
                    {
                        using var sw = File.AppendText($".ms_{configName}.json");
                        sw.WriteLine(ConfigurationTemplate.Template);
                    }

                    var processStartArgs = new ProcessStartInfo("notepad");
                    processStartArgs.Arguments = $"/A \"{filePath}\"";
                    using (var notepadProcess = Process.Start(processStartArgs))
                    {
                        notepadProcess.WaitForExit();
                    }

                    return 1;
                });
            });

            app.Command("run", (cmd) =>
            {
                cmd.OnExecute(() =>
                {
                    var filename = Directory.GetFiles(Directory.GetCurrentDirectory()).FirstOrDefault(f => f.Contains(".ms_"));
                    if (string.IsNullOrEmpty(filename))
                    {
                        using var sw = new StringWriter();
                        sw.WriteLine("No config file found, run \"microstack config\" to setup initial configuration");

                        cmd.Out = sw;
                        return;
                    }

                    var appStack = JsonConvert.DeserializeObject<MicroStack>(File.ReadAllText(filename));
                });
            });

            app.OnExecute(() =>
            {
                if(args.Length <= 0)
                    app.ShowHelp();

                return 0;
            });

            app.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue;


            app.Execute(args);
        }
    }
}
