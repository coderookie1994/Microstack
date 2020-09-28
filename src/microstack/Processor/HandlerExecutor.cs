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
        private StackHandler _registeredHandler = new StartHandler(null);
        private StackHandler _startHandler;

        public HandlerExecutor(IEnumerable<StackHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Execute(IList<Configuration> configurations, bool isVerbose)
        {
            SetupForExecution();
            _ = _startHandler ?? throw new InvalidOperationException("No registered handlers");
            await _startHandler.Handle(configurations, isVerbose);
        }

        private void SetupForExecution()
        {
           for (var i = 0; i < _handlers.Count() - 1; i++)
            {
                var handler = _handlers.ElementAt(i);
                var nextHandler = _handlers.ElementAt(i+1);
                if (_startHandler is null)
                {
                    _startHandler = new StartHandler(null);
                    _startHandler.Next(handler);
                }
                _registeredHandler = handler;
                _registeredHandler.Next(nextHandler);
            }
        }
    }
}