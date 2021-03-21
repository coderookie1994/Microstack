using Microstack.API.Abstractions;
using Microstack.API.Extensions;
using Microstack.Common.Models;
using Microstack.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Services
{
    public class UserService : IUserService
    {
        private readonly IPersistenceProvider _persistenceProvider;

        public UserService(IPersistenceProvider persistenceProvider)
        {
            _persistenceProvider = persistenceProvider;
        }

        public async Task<IList<Profile>> GetProfiles(string userId)
        {
            var profiles = await _persistenceProvider.GetProfilesForUser(userId);
            return profiles.MapFromPersistenceModel();
        }

        public async Task PersistProfile(string userId, Profile profile)
        {
            await _persistenceProvider.PersistProfile(userId, profile.MapToRepositoryModel());
        }
    }
}
