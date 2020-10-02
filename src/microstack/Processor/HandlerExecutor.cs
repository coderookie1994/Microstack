using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using microstack.Abstractions;
using microstack.configuration.Models;

namespace microstack.Processor
{
    /// <summary>
    /// <para>HandlerExecutor executes all the registered handlers registered in <see cref="Program.cs" /></para>
    /// </summary>
    public class HandlerExecutor
    {
        private readonly IEnumerable<StackHandler> _handlers;
        private StackHandler _handlerPointer = new BootstrapHandler(null);
        private StackHandler _bootstrapHandler;

        public HandlerExecutor(IEnumerable<StackHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Execute(IList<Configuration> configurations, bool isVerbose)
        {
            SetupForExecution();
            _ = _bootstrapHandler ?? throw new InvalidOperationException("No registered handlers");
            await _bootstrapHandler.Handle(configurations, isVerbose);
        }

        private void SetupForExecution()
        {
           for (var i = 0; i < _handlers.Count() - 1; i++)
            {
                var handler = _handlers.ElementAt(i);
                var nextHandler = _handlers.ElementAt(i+1);
                if (_bootstrapHandler is null)
                {
                    _bootstrapHandler = new BootstrapHandler(null);
                    _bootstrapHandler.Next(handler);
                }
                _handlerPointer = handler;
                _handlerPointer.Next(nextHandler);
            }
        }
    }
}