using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using microstack.Abstractions;
using microstack.BackgroundTasks;
using microstack.configuration;
using microstack.configuration.Models;
using microstack.git;

namespace microstack.Handlers
{
    public class GitHandler : StackHandler
    {
        private readonly ConfigurationProvider _provider;
        private IGitOps _gitOps;

        public GitHandler(IGitOps gitOps,
            ProcessSpawnManager processSpawnManager,
            ConfigurationProvider configProvider) : base(processSpawnManager, configProvider)
        {
            _gitOps = gitOps;
        }
        public async override Task Handle(bool isVerbose)
        {
            var configurationsWithTemp = configurationProvider.Configurations.Where(c => c.UseTempFs == true);
            
            foreach(var configuration in configurationsWithTemp)
            {
                var gitProjName = configuration.GitUrl.Split('/').LastOrDefault();
                // Check if the project exists in temp
                var dirPath = Environment.ExpandEnvironmentVariables(@"%userprofile%/AppData/Local/Temp");
                if (!Directory.Exists(Path.Combine(dirPath, "MicroStack")))
                    Directory.CreateDirectory(Path.Combine(dirPath, "MicroStack"));
                var dirExists = Directory.EnumerateDirectories(Path.Combine(dirPath, "MicroStack")).Any(d => d.Contains(gitProjName.Replace(".git", string.Empty)));

                if (!dirExists)
                {
                    var newGitRoot = _gitOps.Clone(gitProjName.Replace(".git", string.Empty), configuration.GitUrl, configuration.GitBranchName);
                    configurationProvider.UpdateContext(configuration.ProjectName, newGitRoot);
                }
                else
                {
                    var tempGitPath = Path.Combine(dirPath, "MicroStack", gitProjName.Replace(".git", string.Empty));
                    await _gitOps.PullInTemp(tempGitPath, configuration.GitBranchName);
                    configurationProvider.UpdateContext(configuration.ProjectName, tempGitPath);    
                }
            }            

            await base.Handle(isVerbose);
        }
    }
}