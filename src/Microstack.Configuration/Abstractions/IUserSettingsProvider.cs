using System;
using System.Collections.Generic;
using System.Text;

namespace Microstack.Configuration.Abstractions
{
    public interface IUserSettingsProvider
    {
        string GetSettings();
        void AddSetting(string url);
    }
}
