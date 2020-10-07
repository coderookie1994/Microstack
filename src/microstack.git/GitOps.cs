using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microstack.Configuration;
using Microstack.Git.Abstractions;

namespace Microstack.Git
{
    public class GitOps : IGitOps
    {
        private readonly ICredentialProvider _provider;
        private readonly ConfigurationProvider _configProvider;

        public GitOps(ICredentialProvider provider,
            ConfigurationProvider configProvider)
        {
            _provider = provider;
            _configProvider = configProvider;
        }
        public Task Pull(string gitRoot)
        {
            var gitConfig = _configProvider.Configurations.FirstOrDefault(c => c.GitProjectRootPath.Equals(gitRoot));
            using (var repo = new Repository(gitRoot))
            {
                repo.Network.Remotes.Add(gitConfig.GitBranchName, gitConfig.GitUrl);
                var refSpec = new List<string>() {$"+refs/heads/*:refs/remotes/{gitConfig.GitBranchName}/*"};

                Commands.Fetch(repo, gitConfig.GitBranchName, refSpec, new FetchOptions(){
                    CredentialsProvider = (url, fromUrl, types) => Credentials()
                    }, $"Fetching from {gitConfig.GitBranchName}");

                Commands.Pull(repo,
                    new Signature(_provider.GetCredentials().Username, null, DateTime.Now),
                    new PullOptions()
                    {
                        FetchOptions = new FetchOptions() {
                            CredentialsProvider = 
                                (url, fromUrl, types) => Credentials()
                        }
                    }
                );
            }
            return Task.CompletedTask;
        }

        public Task PullInTemp(string gitRoot, string branchName)
        {
            using(var repo = new Repository(Path.Combine(gitRoot)))
            {
                try {
                    var refSpec = new List<string>() {$"+refs/heads/*:refs/remotes/{branchName}/*"};
                    Commands.Fetch(repo, repo.Network.Remotes.FirstOrDefault().Name, refSpec, new FetchOptions() {
                        CredentialsProvider = (url, fromUrl, types) => Credentials()
                    }, $"Fetching from {branchName}");

                    repo.Reset(ResetMode.Hard);
                    
                    Commands.Checkout(repo, branchName);

                    Commands.Pull(repo,
                        repo.Config.BuildSignature(DateTime.Now),
                        new PullOptions()
                        {
                            FetchOptions = new FetchOptions() {
                                CredentialsProvider = 
                                    (url, fromUrl, types) => Credentials()
                            }
                        }
                    );
                }
                catch(LibGit2Sharp.NotFoundException ex)
                {
                    throw new LibGit2Sharp.NotFoundException("Incorrect branch/url specified, not found");
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException("Incorrect git configuration", ex);
                }
            }

            return Task.CompletedTask;
        }

        public string Clone(string projectName, string remote, string branch)
        {
            var dirPath = Environment.ExpandEnvironmentVariables(@"%userprofile%\AppData\Local\Temp\MicroStack");

            var gitRoot = string.Empty;

            try{
                gitRoot = Repository.Clone(
                    remote, 
                    Path.Combine(dirPath, projectName), 
                    new CloneOptions() 
                    { 
                        BranchName = branch ?? "master", 
                        CredentialsProvider = (url, fromUrl, types) => Credentials()
                    }
                );
                if (gitRoot.EndsWith(@".git\"))
                    gitRoot = gitRoot.Replace(@".git\", string.Empty);
            }
            catch(LibGit2Sharp.NotFoundException ex)
            {
                throw new LibGit2Sharp.NotFoundException("Incorrect branch/url specified, not found");
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Incorrect git configuration", ex);
            }
            
            return gitRoot;
        }

        private UsernamePasswordCredentials Credentials() => 
            new UsernamePasswordCredentials() {
                Username = _provider.GetCredentials().Username, 
                Password = _provider.GetCredentials().Token
            };
    }
}