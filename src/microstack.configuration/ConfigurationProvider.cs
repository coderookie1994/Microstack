using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
        private FileSystemWatcher _fileSystemWatcher;
        private byte[] _lastComputedHash = new byte[]{};

        public bool IsValid { get; private set; }
        public bool IsContextSet { get; private set; }
        public IList<Configuration> Configurations => _selectedConfigurations;
        public event EventHandler<ConfigurationEventArgs> OnConfigurationChange;
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

            // Setup FileSystem events to watch for changes in a profile configuration
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = Path.Combine(Directory.GetCurrentDirectory());
            _fileSystemWatcher.Filter = Path.Combine(_configFile);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
            _fileSystemWatcher.Changed += OnChange;
            _fileSystemWatcher.EnableRaisingEvents = true;
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
                throw new InvalidDataException($"Invalid configuration file format. {ex.Message}");
            }
        }

        private void GetFromEnvironmentPath()
        {
            if(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MSTCK_JSON")))
            {
                var path = Path.Combine(Environment.GetEnvironmentVariable("MSTCK_JSON"));
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Config file not found at {path}");
                _configFile = path;
                ExtractConfigFromPath(path);
            }
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            Dictionary<string, IList<Configuration>> configurations;

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(_configFile);

            // Since FSW fires two events simultaneously, this is a hack to fend away the second event fired, cause the data changes in the first event
            var onHashChange = md5.ComputeHash(stream);
            if (Encoding.UTF8.GetString(onHashChange) == Encoding.UTF8.GetString(_lastComputedHash))
                return;
            _lastComputedHash = onHashChange;

            // Reset stream to read from 0
            stream.Seek(0, SeekOrigin.Begin);
            using var streamReader = new StreamReader(stream);
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                configurations = serializer.Deserialize<Dictionary<string, IList<Configuration>>>(jsonReader);
            }

            EventHandler<ConfigurationEventArgs> handler = OnConfigurationChange;
            ConfigurationEventArgs configChangeArgs;

            if (!string.IsNullOrWhiteSpace(_profile))
                configChangeArgs = new ConfigurationEventArgs(){ UpdatedConfiguration = configurations[_profile] };
            else
                configChangeArgs = new ConfigurationEventArgs() { UpdatedConfiguration = configurations.FirstOrDefault().Value };
            
            if (handler != null)
                handler(null, configChangeArgs);
        }
    }
}