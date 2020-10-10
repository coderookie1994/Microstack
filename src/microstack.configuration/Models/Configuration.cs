using System.Collections.Generic;

namespace Microstack.Configuration.Models
{
    public class Configuration
    {
        public ProcessTypes ProcessType { get; set; }
        public string StartupProjectRelativePath { get; set; }
        public string ProjectName { get; set; }
        public string NextProjectName { get; set; }
        public string GitProjectRootPath { get; set; }
        public string GitUrl { get; set; }
        public string GitBranchName { get; set; }
        public bool PullLatest { get; set; }
        public bool UseTempFs { get; set; }
        public int Port { get; set; }
        public string HostName { get; set; }
        public bool Verbose { get; set; }
        public Dictionary<string, string> ConfigOverrides { get; set; }
        public string StartupDllName { get; set; }

        public (bool IsValid, string Message) Validate()
        {
            if (string.IsNullOrWhiteSpace(StartupProjectRelativePath))
                return (false, $"Required parameter {nameof(StartupProjectRelativePath)} is missing");
            if (string.IsNullOrWhiteSpace(ProjectName))
                return (false, $"Required parameter {nameof(ProjectName)} is missing");
            if (Port == 0)
                return (false, $"Required parameter {nameof(Port)} is missing");
            if (string.IsNullOrWhiteSpace(GitProjectRootPath))
                return (false, $"Required parameter {nameof(GitProjectRootPath)} is missing");
            return (true, string.Empty);
        }

        public (bool IsValid, string Message) ValidateGitConfig()
        {
            if (string.IsNullOrWhiteSpace(GitUrl) && GitUrl.EndsWith(".git"))
                return (false, $"Required parameter {nameof(GitUrl)} is missing");
            if (string.IsNullOrWhiteSpace(GitBranchName))
                return (false, $"Required parameter {nameof(GitBranchName)} is missing");
            
            return (true, string.Empty);
        }
    }

    public enum ProcessTypes
    {
        Dotnet,
        Node
    }
}