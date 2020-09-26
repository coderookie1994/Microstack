using System.Collections.Generic;
using System.Threading.Tasks;
using microstack.Abstractions;
using microstack.configuration.Models;

namespace microstack.Handlers
{
    public class GitHandler : StackHandler
    {
        public async override Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            if (next != null)
                await next.Handle(configurations, isVerbose);
            
            await Task.CompletedTask;
        }
    }
}