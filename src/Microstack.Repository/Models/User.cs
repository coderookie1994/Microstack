using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microstack.Repository.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public IList<Profile> Profiles { get; set; } = new List<Profile>();
    }

    public class Profile
    {
        public string ProfileName { get; set; }
        public IDictionary<string, IList<Configuration.Models.Configuration>> Configurations { get; set; }
    }
}
