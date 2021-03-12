using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        protected List<Process> buildProcs = new List<Process>();
        private int _buildProcCounter;

        public StackHandler(ProcessSpawnManager processSpawnManager,
        ConfigurationProvider configProvider)
        {
            this.processSpawnManager = processSpawnManager;
            this.configurationProvider = configProvider;
        }
        public virtual async Task Handle(bool isVerbose)
        {
            while (_buildProcCounter > 0) { }
            PostHandle();
            if (next != null)
            {
                next.PreHandle();
                await next.Handle(isVerbose);
            }
        }

        public virtual void PreHandle()
        {
            _buildProcCounter = buildProcs.Count();
            var lockObj = new object();
            foreach(var buildProcess in buildProcs)
            {
                buildProcess.EnableRaisingEvents = true;
                buildProcess.Exited += (sender, args) => {
                    lock(lockObj) {
                        _buildProcCounter--;
                    }
                };
            }
        }

        public virtual void PostHandle()
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

        public override async Task Handle(bool isVerbose)
        {
            await base.Handle(isVerbose);    
        }
    }
}