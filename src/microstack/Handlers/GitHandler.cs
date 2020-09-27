using System.Collections.Generic;
using System.Threading.Tasks;
using microstack.Abstractions;
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
            IGitOps gitOps)
        {
            _provider = provider;
            _gitOps = gitOps;
        }
        public async override Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            _provider.SetConfigurations(configurations);

            // if (next != null)
            //     await next.Handle(configurations, isVerbose);
            
            await base.Handle(configurations, isVerbose);
        }

        private void Validate(IList<Configuration> configurations)
        {

        }
    }
}