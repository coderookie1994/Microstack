using System.Collections.Generic;

namespace microstack.Models
{
    public class Configuration
    {
        public string StartupProjectPath { get; set; }
        public string ProjectName { get; set; }
        public string GitProjectRootPath { get; set; }
        public bool PullLatest { get; set; }
        public int Port { get; set; }
        public Dictionary<string, string> ConfigOverrides { get; set; }
    }
}