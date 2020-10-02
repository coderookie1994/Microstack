using System;
using System.Collections.Generic;

namespace microstack.configuration.Models
{
    public class ConfigurationEventArgs : EventArgs
    {
        public IList<Configuration> UpdatedConfiguration;
    }
}