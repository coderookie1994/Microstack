using System.Collections.Generic;
using System.Threading.Tasks;
using microstack.configuration.Models;

namespace microstack.Abstractions
{
    public abstract class StackHandler
    {
        protected StackHandler next;
        public abstract Task Handle(IList<Configuration> configurations, bool isVerbose);
        public void Next(StackHandler next) => this.next = next;

        public StackHandler NextHandler => next;
    }

    public class StartHandler : StackHandler
    {
        public async override Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            await next.Handle(configurations, isVerbose);    
        }
    }
}