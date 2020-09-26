using System;
using System.Collections.Generic;
using System.Text;

namespace microstack.Models
{
    public class ConfigurationTemplate
    {
        public static string Template = @"{
	""apps"": 
	[
		{
			""appPath"": ""<<YOUR LOCAL APPLICATION PROJECT PATH>>"",
            ""environmentVariables"": {
			""<<YOUR KEY>>"": ""<<YOUR VALUE>>""
            }
        }
    ]
}
";
    }
}
