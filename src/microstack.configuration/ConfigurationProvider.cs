using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using microstack.configuration.Models;
using Newtonsoft.Json;

namespace microstack.configuration
{
    public class ConfigurationProvider
    {
        private Dictionary<string, IList<Configuration>> _configurations;
        private string _configFile;
        private string _profile;
        private IList<Configuration> _selectedConfigurations;

        public bool IsValid { get; private set; }
        public bool IsContextSet { get; private set; }

        public IList<Configuration> Configurations => _selectedConfigurations;

        public void SetContext(string configFile, string profile)
        {
            // Extract config from either path or env
            // Path takes priority
            _configFile = configFile;
            _profile = profile;
            if (configFile != null)
            {
                ExtractConfigFromPath(configFile);
            }
            else
            {
                GetFromEnvironmentPath();
            }
            IsContextSet = true;
        }

        public (int ReturnCode, string Message) Validate()
        {
            if (!IsContextSet)
                throw new InvalidOperationException("Context not set");

            if (_configurations.Count > 0 && string.IsNullOrWhiteSpace(_profile))
                return (1, "Multiple profiles found use -p to specify profile to use");
            else if (!_configurations.ContainsKey(_profile))
                return (1, $"Profile {_profile} not found in configuration");

            return (0, string.Empty);
        }

        public void SetConfigurationContext()
        {
            if (!string.IsNullOrWhiteSpace(_profile))
                _selectedConfigurations = _configurations[_profile];
            else if (_configurations.Count == 1)
                _selectedConfigurations = _configurations.FirstOrDefault().Value;
        }

        private void ExtractConfigFromPath(string path)
        {       
            try {
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, IList<Configuration>>>(File.ReadAllText(Path.Combine(path)));
                foreach(var profile in _configurations)
                {
                    var validationResult = profile.Value.Select(c => c.Validate());
                    var hasDistinctProjectNames = profile.Value.Select(c => c.ProjectName).Distinct().Count() == profile.Value.Count();

                    if (validationResult.Any(v => v.IsValid == false))
                        throw new InvalidDataException();
                    if (!hasDistinctProjectNames)
                        throw new InvalidDataException($"Project names in a profile have to be unique");
                }
            } catch(Exception ex)
            {
                throw new InvalidDataException($"Invalid configuration file format {ex.Message}");
            }
        }

        private void GetFromEnvironmentPath()
        {
            if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSTCK_JSON")))
            {
                var path = Path.Combine(Environment.GetEnvironmentVariable("MSTCK_JSON"));
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Config file not found at {path}");
                ExtractConfigFromPath(path);
            }
        }
    }
}