using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microstack.CLI.Processor;
using Xunit;
using Moq;
using Microstack.CLI.Handlers;
using Microstack.CLI.Extensions;
using Microstack.CLI.Abstractions;
using System.Collections;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using Microstack.Configuration.Models;
using Microstack.Git.Abstractions;
using Microstack.CLI.BackgroundTasks;

namespace Microstack.tests
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
            Func<Task> action = async () => await executor.Execute(false);

            // Then
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("No registered handlers");            
        }

        [Fact]
        public async Task Execute_WhenHandlersAreRegistered_ShouldExecute()
        {
            // Given
            var configurationProvider = new Mock<Microstack.Configuration.ConfigurationProvider>(new Mock<IConsole>().Object);
            var gitHandler = new Mock<GitHandler>(new Mock<IGitOps>().Object,
                new Mock<Microstack.CLI.BackgroundTasks.ProcessSpawnManager>().Object,
                configurationProvider.Object);
            var processHandler = new Mock<DotnetHandler>(new Mock<IConsole>().Object, new Mock<ProcessSpawnManager>().Object, configurationProvider.Object);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConsole>(new Mock<IConsole>().Object);
            serviceCollection.RegisterHandlers(sh => {
                sh.AddHandler<StackHandler>(gitHandler.Object);
                sh.AddHandler<StackHandler>(processHandler.Object);
            });
            serviceCollection.AddSingleton<HandlerExecutor>();
            
            var provider = serviceCollection.BuildServiceProvider();
            var executor = provider.GetRequiredService<HandlerExecutor>();

            // When
            await executor.Execute(false);

            // Then
            gitHandler.Verify(h => h.Handle(It.IsAny<bool>()), Times.Once);
        }
    }
}