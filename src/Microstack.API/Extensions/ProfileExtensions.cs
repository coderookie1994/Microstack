using Microstack.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Extensions
{
    public static class ProfileExtensions
    {
        public static IList<Profile> MapFromPersistenceModel(this List<Microstack.Repository.Models.Profile> profiles)
        {
            var result = new List<Profile>();
            foreach(var p in profiles)
            {
                result.Add(new Profile()
                {
                    FileName = p.ProfileName,
                    Configurations = p.Configurations
                });
            }
            return result;
        }

        public static Microstack.Repository.Models.Profile MapToRepositoryModel(this Profile profile)
        {
            var result = new Repository.Models.Profile();
            result.ProfileName = profile.FileName;
            result.Configurations = profile.Configurations;
            return result;
        }
    }
}
