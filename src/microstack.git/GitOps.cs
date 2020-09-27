using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;
using microstack.configuration;
using microstack.git.Abstractions;

namespace microstack.git
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
            var gitConfig = _configProvider.Configuration(c => c.GitProjectRootPath = gitRoot);
            using (var repo = new Repository(gitRoot))
            {
                repo.Network.Remotes.Add(gitConfig.GitRemoteName, gitConfig.GitUrl);
                var refSpec = new List<string>() {$"+refs/heads/*:refs/remotes/{gitConfig.GitRemoteName}/*"};

                Commands.Fetch(repo, gitConfig.GitRemoteName, refSpec, new FetchOptions(){
                    CredentialsProvider = (url, fromUrl, types) => Credentials()
                    }, $"Fetching from {gitConfig.GitRemoteName}");

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

        private UsernamePasswordCredentials Credentials() => 
            new UsernamePasswordCredentials() {
                Username = _provider.GetCredentials().Username, 
                Password = _provider.GetCredentials().Token
            };
    }
}