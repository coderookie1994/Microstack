using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microstack.CLI.BackgroundTasks;
using Microstack.Configuration;

namespace Microstack.CLI.Abstractions
{
    public abstract class StackHandler
    {
        protected StackHandler next;
        protected ProcessSpawnManager processSpawnManager;
        protected ConfigurationProvider configurationProvider;
        protected bool raiseEventOnHandleComplete;

        public StackHandler(ProcessSpawnManager processSpawnManager,
        ConfigurationProvider configProvider)
        {
            this.processSpawnManager = processSpawnManager;
            this.configurationProvider = configProvider;
        }
        public virtual async Task Handle(bool isVerbose)
        {
            OnHandleComplete();
            if (next != null)
                await next.Handle(isVerbose);
        }

        public virtual void OnHandleComplete()
        {

        }
        public void Next(StackHandler next) => this.next = next;

        public StackHandler NextHandler => next;
    }

    public class BootstrapHandler : StackHandler
    {
        public BootstrapHandler(ProcessSpawnManager processSpawnManager, ConfigurationProvider configProvider) : base(processSpawnManager, configProvider)
        {
        }

        public async override Task Handle(bool isVerbose)
        {
            await base.Handle(isVerbose);    
        }
    }
}