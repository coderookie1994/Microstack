using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using microstack.Abstractions;
using microstack.configuration.Models;
using microstack.Extensions;

namespace microstack.Processor
{
    public class HandlerExecutor
    {
        private readonly IEnumerable<StackHandler> _handlers;
        private StackHandler _registeredHandler = new StartHandler();
        private StackHandler _startHandler;

        public HandlerExecutor(IEnumerable<StackHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Execute(IList<Configuration> configurations, bool isVerbose)
        {
            PrimeForExecution();
            _ = _startHandler ?? throw new InvalidOperationException("No registered handlers");
            await _startHandler.Handle(configurations, isVerbose);
        }

        private void PrimeForExecution()
        {
           for (var i = 0; i < _handlers.Count() - 1; i++)
            {
                var handler = _handlers.ElementAt(i);
                var nextHandler = _handlers.ElementAt(i+1);
                if (_startHandler is null)
                {
                    _startHandler = new StartHandler();
                    _startHandler.Next(handler);
                }
                _registeredHandler = handler;
                _registeredHandler.Next(nextHandler);
            }
        }
    }
}