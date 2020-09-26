using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using microstack.Abstractions;
using microstack.Processor;

namespace microstack.Extensions
{
    public static class ExecutorExtensions
    {
        public static IServiceCollection RegisterHandlers(this IServiceCollection services, Action<IServiceCollection> options)
        {
            _ = options ?? throw new ArgumentNullException("Handlers cannot be null");
            options(services);

            return services;
        }
    }
}