using System.Collections.Generic;
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

        public GitHandler(ConfigurationProvider provider,
            IGitOps gitOps,
            ProcessSpawnManager processSpawnManager,
            ConfigurationProvider configProvider) : base(processSpawnManager, configProvider)
        {
            _provider = provider;
            _gitOps = gitOps;
        }
        public async override Task Handle(bool isVerbose)
        {
            // if (next != null)
            //     await next.Handle(configurations, isVerbose);
            
            await base.Handle(isVerbose);
        }

        private void Validate(IList<Configuration> configurations)
        {

        }
    }
}