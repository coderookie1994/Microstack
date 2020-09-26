using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using microstack.Processor;
using Xunit;
using Moq;
using microstack.Handlers;
using microstack.Extensions;
using microstack.Abstractions;
using System.Collections;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace microstack.tests
{
    public class ExecutorSpecs
    {
        [Fact]
        public async Task Execute_WhenNoHandlersAreRegistered_ShouldThrowInvalidOperaqtionException()
        {
            // Given
            var serviceCollection = new ServiceCollection();
            var provider = serviceCollection.BuildServiceProvider();
            var stackHandlers = provider.GetServices<StackHandler>();
            var executor = new HandlerExecutor(stackHandlers);

            // When
            Func<Task> action = async () => await executor.Execute(null, false);

            // Then
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("No registered handlers");            
        }

        [Fact]
        public async Task Execute_WhenHandlersAreRegistered_ShouldExecute()
        {
            // Given
            var gitHandler = new Mock<GitHandler>();
            var processHandler = new Mock<ProcessHandler>(new Mock<IConsole>().Object);
            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterHandlers(sc => {
                sc.AddSingleton<StackHandler>(gitHandler.Object);
                sc.AddSingleton<StackHandler>(processHandler.Object);
            });
            serviceCollection.AddSingleton<HandlerExecutor>();
            
            var provider = serviceCollection.BuildServiceProvider();
            var executor = provider.GetRequiredService<HandlerExecutor>();

            // When
            await executor.Execute(null, false);

            // Then
            gitHandler.Verify(h => h.Handle(It.IsAny<IList<microstack.configuration.Models.Configuration>>(), It.IsAny<bool>()), Times.Once);
        }
    }
}