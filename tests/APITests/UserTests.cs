using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microstack.API;
using Microstack.Common.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microstack.Tests.API_Tests
{
    public class UserTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;

        public UserTests(WebApplicationFactory<Startup> app)
        {
            app.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.AddJsonFile(Path.Combine(".", "config", "appSettings.test.json"));
                }).Build();
            });
            var s = app.Services.GetRequiredService<IMongoClient>();
            _client = app.CreateClient();
        }

        [Fact]
        public async Task Post_ShouldReturnNoContent()
        {
            var config = JsonConvert.DeserializeObject<IDictionary<string, IList<Configuration.Models.Configuration>>>(File.ReadAllText(Path.Combine("config", ".mstkc.json")));
            var profile = new Profile
            {
                FileName = "mstkc",
                Configurations = config
            };
            using (var message = new HttpRequestMessage(HttpMethod.Post, "api/users/sharthak_ghosh/profile"))
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
                var result = await _client.SendAsync(message);
                result.StatusCode.Should().Be(204);
            }
        }

        [Fact]
        public async Task Get_ShouldReturn404WhenUserNotFound()
        {
            using (var message = new HttpRequestMessage(HttpMethod.Get, "api/users/ghosh_sharthak"))
            {
                var result = await _client.SendAsync(message);
                result.StatusCode.Should().Be(404);
            }
        }
    }
}
