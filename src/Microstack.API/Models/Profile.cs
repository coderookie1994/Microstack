using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microstack.API.Models
{
    public class Profile
    {
        public string FileName { get; set; }
        public IDictionary<string, IList<Configuration.Models.Configuration>> Configurations { get; set; }
    }
}
