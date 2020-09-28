using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using microstack.BackgroundTasks;
using microstack.configuration.Models;

namespace microstack.Abstractions
{
    public abstract class StackHandler
    {
        protected StackHandler next;
        protected ProcessSpawnManager processSpawnManager;

        protected bool raiseEventOnHandleComplete;

        public StackHandler(ProcessSpawnManager processSpawnManager)
        {
            this.processSpawnManager = processSpawnManager;
        }
        public virtual async Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            OnHandleComplete();
            if (next != null)
                await next.Handle(configurations, isVerbose);
        }

        public virtual void OnHandleComplete()
        {

        }
        public void Next(StackHandler next) => this.next = next;

        public StackHandler NextHandler => next;
    }

    public class StartHandler : StackHandler
    {
        public StartHandler(ProcessSpawnManager processSpawnManager) : base(processSpawnManager)
        {
        }

        public async override Task Handle(IList<Configuration> configurations, bool isVerbose)
        {
            await base.Handle(configurations, isVerbose);    
        }
    }
}