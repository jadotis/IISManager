using IISSetup.Code;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IISSetup
{
    class Program
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            string dir = Directory.GetCurrentDirectory();
            BasicConfigurator.Configure();
            if (Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled))
            {
                //Returns the static path for debugging purposes.
                dir = "C:\\Users\\JOTIS\\source\\repos\\IISSetup";
            }
            Settings settings = new Settings();
            var builder = new ConfigurationBuilder()
                .SetBasePath(dir)
                .AddJsonFile("App_Data/appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            configuration.GetSection("AppSettings").Bind(settings);

            WindowsIdentity user = WindowsIdentity.GetCurrent(); 
            if(user.IsGuest || user.IsAnonymous)
            {
                Console.WriteLine("User is currently an ninvalid user....");
                Environment.Exit(-1);
            }
            
            foreach(var application in settings.Applications)
            {
                IISManager manager = new IISManager(application);
                manager.CreateInstance();

            }

        }
    }
}
