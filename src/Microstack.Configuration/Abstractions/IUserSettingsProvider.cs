using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.Configuration.Abstractions
{
    public interface IUserSettingsProvider
    {
        string GetSettings();
        void AddSetting(string url);
        Task<(string Response, bool Error)> GetUserSettings(string userId);
        Task<(string Response, bool Error)> ListAllUsers();
    }
}
