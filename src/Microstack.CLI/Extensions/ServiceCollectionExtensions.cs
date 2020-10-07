using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microstack.CLI.Abstractions;
using Microstack.CLI.BackgroundTasks;
using Microstack.CLI.Processor;

namespace Microstack.CLI.Extensions
{
    public static class ExecutorExtensions
    {
        ///<summary>
        ///<para>Registers handlers that will be executed in sequence of registration by the executor</para>
        ///</summary>
        public static IServiceCollection RegisterHandlers(this IServiceCollection services, Action<HandlerSetup> handlerAction)
        {
            services.AddSingleton<ProcessSpawnManager>();
            _ = handlerAction ?? throw new ArgumentNullException("Handlers cannot be null");
            var setupHandlers = new HandlerSetup();
            handlerAction(setupHandlers);

            foreach(var handlerType in setupHandlers.Handlers)
            {
                services.AddSingleton(typeof(StackHandler), handlerType);
            }
            
            foreach(var handlerimpl in setupHandlers.HandlerImpl)
            {
                services.AddSingleton<StackHandler>(handlerimpl);
            }
            
            return services;
        }
    }

    public class HandlerSetup
    {
        private IList<Type> _handlers = new List<Type>();
        private IList<StackHandler> _handlerImpl = new List<StackHandler>();
        public void AddHandler<T>() where T : StackHandler
        {
            if (_handlers.Contains(typeof(T)))
                throw new ArgumentException($"Handler nameof{typeof(T)} already registered");
            _handlers.Add(typeof(T));
        }

        public void AddHandler<T>(T handlerImpl) where T: StackHandler
        {
            if (_handlerImpl.Contains(handlerImpl))
                throw new ArgumentException("Handler already registered");
            _handlerImpl.Add(handlerImpl);
        }

        public IList<StackHandler> HandlerImpl => _handlerImpl;
        public IList<Type> Handlers => _handlers;
    }
}