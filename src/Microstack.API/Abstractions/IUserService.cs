using Microstack.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Abstractions
{
    public interface IUserService
    {
        public Task<IList<Profile>> GetProfiles(string userId);
    }
}
