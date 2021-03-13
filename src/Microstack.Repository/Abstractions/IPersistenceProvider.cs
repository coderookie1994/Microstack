using Microstack.Repository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microstack.Repository.Abstractions
{
    public interface IPersistenceProvider
    {
        Task<List<Profile>> GetProfilesForUser(string userId);
    }
}
