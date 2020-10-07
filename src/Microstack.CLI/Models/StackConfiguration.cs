using System.Collections.Generic;

namespace Microstack.CLI.Models
{
    public class StackConfiguration
    {
        public string ProjectName { get; set; }
        public int Port { get; set; }
        public Dictionary<string, string> ConfigOverrides { get; set; }
        public StackConfiguration NextProject { get; set; }    
    }
}