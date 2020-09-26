using System;
using System.Collections.Generic;
using microstack.Abstractions;

namespace microstack.Extensions
{
    public class HandlerOptions
    {
        IList<Type> _handlerTypes;
        public HandlerOptions()
        {
            _handlerTypes = new List<Type>();
        }
        
        public void AddHandler<T>() where T : StackHandler
        {
            _handlerTypes.Add(typeof(T));
        }

        public IList<Type> HandlerTypes => _handlerTypes;
    }
}