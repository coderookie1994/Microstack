using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using microstack.configuration.Models;
using Newtonsoft.Json;
using McMaster.Extensions.CommandLineUtils;

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
        private Timer _watcherThread;
        private object _lockObj = new object();
        private IConsole _console;
        private long _lastWrite;

        public bool IsValid { get; private set; }
        public bool IsContextSet { get; private set; }
        public IList<Configuration> Configurations => _selectedConfigurations;
        public event EventHandler<ConfigurationEventArgs> OnConfigurationChange;

        public ConfigurationProvider(IConsole console)
        {
            _console = console;
        }
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
            Console.WriteLine("Setting up watcher");
            
            // _fileSystemWatcher = new FileSystemWatcher();
            // _fileSystemWatcher.Path = Path.Combine(Directory.GetCurrentDirectory());
            // _fileSystemWatcher.Filter = Path.Combine(_configFile);
            // _fileSystemWatcher.Changed += WatchForChange;
            // _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Attributes;
            // _fileSystemWatcher.EnableRaisingEvents = true;
            _watcherThread = new Timer(WatchForChange, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
        }

        private void ExtractConfigFromPath(string path)
        {       
            try {
                _configurations = JsonConvert.DeserializeObject<Dictionary<string, IList<Configuration>>>(File.ReadAllText(Path.Combine(path)));
                // _lastComputedHash = ComputeHash(path);
                ValidateProfileAndConfigurations();
                _lastWrite = File.GetLastWriteTime(_configFile).ToFileTimeUtc();
            } catch(Exception ex)
            {
                throw new InvalidDataException($"Invalid configuration file format. {ex.Message}");
            }
        }

        private void ValidateProfileAndConfigurations()
        {
            foreach(var profile in _configurations)
            {
                var validationResult = profile.Value.Select(c => c.Validate());
                var hasDistinctProjectNames = profile.Value.Select(c => c.ProjectName).Distinct().Count() == profile.Value.Count();

                if (validationResult.Any(v => v.IsValid == false))
                    throw new InvalidDataException();
                if (!hasDistinctProjectNames)
                    throw new InvalidDataException($"Project names in a profile have to be unique");
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

        private void WatchForChange(object state)
        {
            lock (_lockObj) {
                try {
                    var onChange = File.GetLastWriteTime(_configFile).ToFileTimeUtc();
                    // var hashOnChange = ComputeHash(_configFile);
                    if (onChange == _lastWrite)
                        return;

                    _lastWrite = onChange;
                    Console.WriteLine("Trigerred");

                    using var stream = File.OpenRead(_configFile);
                    using var streamReader = new StreamReader(stream);
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        _configurations = serializer.Deserialize<Dictionary<string, IList<Configuration>>>(jsonReader);
                        
                        try {
                            ValidateProfileAndConfigurations();
                        } catch(Exception ex)
                        {
                            _console.ForegroundColor = ConsoleColor.DarkRed;
                            _console.Out.WriteLine($"Invalid configuration found. {ex.Message}");
                            _console.ResetColor();
                            return;
                        }
                    }
                } catch(Exception)
                { 
                    return; 
                }
                finally
                {

                }

                EventHandler<ConfigurationEventArgs> handler = OnConfigurationChange;
                ConfigurationEventArgs configChangeArgs;

                if (!string.IsNullOrWhiteSpace(_profile))
                    configChangeArgs = new ConfigurationEventArgs(){ UpdatedConfiguration = _configurations[_profile] };
                else
                    configChangeArgs = new ConfigurationEventArgs() { UpdatedConfiguration = _configurations.FirstOrDefault().Value };
                
                if (handler != null)
                    handler(null, configChangeArgs);
            }
        }

        // private byte[] ComputeHash(string path)
        // {
        //     using var md5 = MD5.Create();
        //     var stream = File.OpenRead(path);
        //     var hash = md5.ComputeHash(stream);
        //     return hash;
        // }
    }
}