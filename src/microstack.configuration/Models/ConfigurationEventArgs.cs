using System;
using System.Collections.Generic;

namespace Microstack.Configuration.Models
{
    public class ConfigurationEventArgs : EventArgs
    {
        public IList<Configuration> UpdatedConfiguration;
    }
}