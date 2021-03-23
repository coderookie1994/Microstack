using Microstack.Configuration.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.Configuration
{
    public class UserSettingsProvider : IUserSettingsProvider
    {
        public void AddSetting(string url)
        {
            var dirPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "AppData", "Local", ".microstack");
            File.WriteAllText(dirPath, url);
        }

        public string GetSettings()
        {
            var dirPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "AppData", "Local", ".microstack");
            if (!File.Exists(dirPath))
                return string.Empty;

            return File.ReadAllText(dirPath);
        }

        public async Task<(string Response, bool Error)> GetUserSettings(string userId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(GetSettings());
                try
                {
                    var response = await client.GetAsync($"/api/users/{userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()), Formatting.Indented);
                        return (formattedJson, false);
                    }
                    return (await response.Content.ReadAsStringAsync(), true);
                }
                catch (Exception ex)
                {
                    return ($"Error occurred while connecting to api {ex.Message}", true);
                }
            }
        }

        public async Task<(string Response, bool Error)> ListAllUsers()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(GetSettings());
                try
                {
                    var response = await client.GetAsync($"/api/users");
                    if (response.IsSuccessStatusCode)
                    {
                        var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()), Formatting.Indented);
                        return (formattedJson, false);
                    }
                    return (await response.Content.ReadAsStringAsync(), true);
                }
                catch (Exception ex)
                {
                    return ($"Error occurred while connecting to api {ex.Message}", true);
                }
            }
        }
    }
}
