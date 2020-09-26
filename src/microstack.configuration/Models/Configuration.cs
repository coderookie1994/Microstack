using System.Collections.Generic;

namespace microstack.configuration.Models
{
    public class Configuration
    {
        public string StartupProjectPath { get; set; }
        public string ProjectName { get; set; }
        public string NextProjectName { get; set; }
        public string GitProjectRootPath { get; set; }
        public string GitUrl { get; set; }
        public string GitRemoteName { get; set; }
        public bool PullLatest { get; set; }
        public bool InMemoryGitFS { get; set; }
        public int Port { get; set; }
        public bool Verbose { get; set; }
        public Dictionary<string, string> ConfigOverrides { get; set; }

        public (bool IsValid, string Message) Validate()
        {
            if (string.IsNullOrWhiteSpace(StartupProjectPath))
                return (false, $"Required parameter {nameof(StartupProjectPath)} is missing");
            if (string.IsNullOrWhiteSpace(ProjectName))
                return (false, $"Required parameter {nameof(ProjectName)} is missing");
            if (Port == 0)
                return (false, $"Required parameter {nameof(Port)} is missing");
            if (string.IsNullOrWhiteSpace(GitProjectRootPath))
                return (false, $"Required parameter {nameof(GitProjectRootPath)} is missing");
            return (true, string.Empty);
        }
    }
}