using System;
using System.Collections.Generic;
using System.Text;

namespace microstack
{
    public class Application
    {
        public string AppDirPath { get; set; }
        public Dictionary<string, string> EnvironmentVariables { get; set; }
    }
}
