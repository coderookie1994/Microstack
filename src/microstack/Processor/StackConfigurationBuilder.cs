// using System.Collections.Generic;
// using System.Linq;
// using microstack.Models;

// namespace microstack.Processor
// {
//     public class StackConfigurationBuilder
//     {
//         private List<Configuration> _configurations;
//         private IList<StackConfiguration> _stackConfig;

//         private int StartingPort = 10000;

//         public StackConfigurationBuilder(IList<Configuration> configurations)
//         {
//             _configurations = configurations.Reverse().ToList();
            
//         }

//         public void Build()
//         {

//         }

//         private void Recurse(int index)
//         {
//             if (_configurations[index].NextProjectName != null)
//             {
//                 var nextIdx = _configurations
//                     .FindIndex(c => c.ProjectName.Equals(_configurations[index].NextProjectName));
                
//             }
//         }
//     }
// }