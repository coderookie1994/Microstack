using System;
using System.Collections.Generic;
using System.Linq;
using microstack.configuration.Models;

namespace microstack.configuration
{
    public class ConfigurationProvider
    {
        private IList<Configuration> _configurations;

        public void SetConfigurations(IList<Configuration> configurations) 
            => _configurations = configurations;

        public Configuration Configuration(Action<Configuration> searchOptions)
        {
            var options = new Configuration();
            searchOptions(options);
            return _configurations.FirstOrDefault(c => c.GitProjectRootPath == options.GitProjectRootPath);
        }

        public IList<Configuration> Configurations => _configurations;
    }
}