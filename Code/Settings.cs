using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IISSetup.Code
{
    class Settings
    {
        public List<Application> Applications { get; set; }

    }


    public class Application
    {
        public string ApplicationName { get; set; }
        public string ApplicationPool { get; set; }
        public string RuntimeVersion { get; set; }
        public string PipelineMode { get; set; }
        public string Path { get; set; }
        public int Port { get; set; }
    }
}
