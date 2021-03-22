using Microsoft.AspNetCore.Mvc;
using Microstack.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Abstractions
{
    public interface IUserService
    {
        public Task<IList<Profile>> GetProfiles(string userId);
        public Task PersistProfile(string userId, Profile profile);
        Task<IReadOnlyList<string>> GetUsers();
        Task<Profile> GetProfile(string userId, string profileId);
    }
}
