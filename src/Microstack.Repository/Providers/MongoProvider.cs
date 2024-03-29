﻿using Microstack.Repository.Abstractions;
using Microstack.Repository.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
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

        public async Task PersistProfile(string userId, Profile profile)
        {
            var filter = Builders<User>.Filter.Eq(f => f.UserId, userId);
            var userProfiles = _database.GetCollection<User>("user.profiles");
            var update = Builders<User>.Update.AddToSet<Profile>(p => p.Profiles, profile);
            var arrayFilter = filter & Builders<User>.Filter.ElemMatch(p => p.Profiles, Builders<Profile>.Filter.Eq(f => f.ProfileName, profile.ProfileName));
            var exists = (await userProfiles.FindAsync<User>(arrayFilter)).ToList();
            
            if (exists.Any())
            {
                var positionalUpdate = Builders<User>.Update.Set<Profile>(p => p.Profiles[-1], profile);
                await userProfiles.UpdateOneAsync(arrayFilter, positionalUpdate);
                return;
            }

            await userProfiles.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        public async Task<IReadOnlyList<string>> GetUsers()
        {
            var userProfiles = _database.GetCollection<User>("user.profiles");
            var users = await (userProfiles.Find(FilterDefinition<User>.Empty).Project(f => f.UserId).ToListAsync());
            return users;
        }

        public async Task<Profile> GetProfile(string userId, string profileName)
        {
            var filter = Builders<User>.Filter.Eq(f => f.UserId, userId);
            var userProfiles = _database.GetCollection<User>("user.profiles");
            var result = (await userProfiles.FindAsync<User>(filter)).ToList().SelectMany(u => u.Profiles.Where(p => p.ProfileName.Equals(profileName)));
            throw new System.NotImplementedException();
        }
    }
}
