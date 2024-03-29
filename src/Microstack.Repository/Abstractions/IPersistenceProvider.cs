﻿using Microstack.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.Repository.Abstractions
{
    public interface IPersistenceProvider
    {
        Task<List<Profile>> GetProfilesForUser(string userId);
        Task PersistProfile(string userId, Profile profile);
        Task<IReadOnlyList<string>> GetUsers();
        Task<Profile> GetProfile(string userId, string profileName);
    }
}
