using Microstack.Repository.Abstractions;
using Microstack.Repository.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.Repository.Providers
{
    public class MongoProvider : IPersistenceProvider
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoProvider(IMongoClient client)
        {
            _client = client;
            _database = _client.GetDatabase("microstack");
        }

        public async Task<List<Profile>> GetProfilesForUser(string userId)
        {
            var filter = Builders<User>.Filter.Eq(f => f.UserId, userId);
            var userProfiles = _database.GetCollection<User>("user.profiles");
            var result = (await userProfiles.FindAsync<User>(filter)).ToList();
            return result.SelectMany(r => r.Profiles).ToList();
        }
    }
}
