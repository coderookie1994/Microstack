using Microstack.Configuration.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
