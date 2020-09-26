using System.Collections.Generic;
using System.Linq;
using microstack.configuration.Models;
using microstack.Models;

namespace microstack.Builders
{
    public class StackConfigBuilder
    {
        private IList<Configuration> _configurations;
        private IList<StackConfiguration> _stackConfig;
        private Dictionary<string, bool> _visited;
        public StackConfigBuilder(IList<Configuration> configurations)
        {
            _visited = new Dictionary<string, bool>();
            _stackConfig = new List<StackConfiguration>();
            _configurations = configurations;
        }

        public void Build()
        {
            _configurations.Reverse();
        }
    }
}