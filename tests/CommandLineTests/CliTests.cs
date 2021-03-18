using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microstack.CLI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microstack.Tests.CommandLineTests
{
    public class CliTests : IDisposable
    {
        private bool disposedValue;
        private CancellationTokenSource _cts;
        private HttpClient _client;

        public CliTests()
        {
            _cts = new CancellationTokenSource();
            _client = new HttpClient();
        }

        [Fact]
        public async Task ShouldOverrideConfigValues_WhenSpecifiedInConfiguration()
        {
            var cts = new CancellationTokenSource();
            var testAdapter = new ProgramTest();
            var daemonTask = testAdapter.StartDaemon();
            var cliHost = testAdapter.StartHost(new string[] { "run", "-v", "-c", Path.Combine("config", ".mstkc.json"), "-p", "profile2" }, cts);

            Thread.Sleep(1000 * 5);

            var response1 = await (await _client.GetAsync("https://localhost:5004/appsettings/ComplexObj:ComplexKey")).Content.ReadAsStringAsync();
            var response2 = await (await _client.GetAsync("https://localhost:5005/appsettings/ComplexObj:ComplexKey")).Content.ReadAsStringAsync();

            Assert.Equal("app1", response1);
            Assert.Equal("app2", response2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cts.Cancel();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CliTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
