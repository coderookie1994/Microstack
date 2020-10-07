using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microstack.CLI.Abstractions;
using Microstack.CLI.Models;

namespace Microstack.CLI.Processor
{
    /// <summary>
    /// <para>HandlerExecutor executes all the registered handlers registered in <see cref="Program.cs" /></para>
    /// </summary>
    public class HandlerExecutor
    {
        private readonly IEnumerable<StackHandler> _handlers;
        private StackHandler _handlerPointer = new BootstrapHandler(null, null);
        private StackHandler _bootstrapHandler;

        public HandlerExecutor(IEnumerable<StackHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task Execute(bool isVerbose)
        {
            SetupForExecution();
            _ = _bootstrapHandler ?? throw new InvalidOperationException("No registered handlers");
            await _bootstrapHandler.Handle(isVerbose);
        }

        private void SetupForExecution()
        {
           for (var i = 0; i < _handlers.Count() - 1; i++)
            {
                var handler = _handlers.ElementAt(i);
                var nextHandler = _handlers.ElementAt(i+1);
                if (_bootstrapHandler is null)
                {
                    _bootstrapHandler = new BootstrapHandler(null, null);
                    _bootstrapHandler.Next(handler);
                }
                _handlerPointer = handler;
                _handlerPointer.Next(nextHandler);
            }
        }
    }
}