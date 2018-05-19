using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IISSetup.Code
{
    class IISManager
    {
        private Application App { get; set; }
        public ServerManager manager = new ServerManager();
        public IISManager(Application app)
        {
            App = app;
        }
        public bool CreateInstance()
        {
            //Modifies the ApplicationHost.Config for IIS
            
            if (manager.Sites.Where(r => r.Name == App.ApplicationName).ToList().Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"There already exists an application with the name {App.ApplicationName}...");
                Console.ResetColor();
                return false;
            }
            try
            {
                //Adds the website to IIS
                Site webSite = manager.Sites.Add(App.ApplicationName, App.Path, App.Port);
                webSite.ServerAutoStart = true;

                manager.ApplicationPools.Add(App.ApplicationPool);
                manager.Sites[App.ApplicationName].Applications.First().ApplicationPoolName = App.ApplicationPool;
                ApplicationPool pool = manager.ApplicationPools[App.ApplicationPool];
                pool.ManagedRuntimeVersion = App.RuntimeVersion;
                if (App.PipelineMode.ToLower() == "classic")
                {
                    pool.ManagedPipelineMode = ManagedPipelineMode.Classic;
                }
                else
                {
                    pool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                }
                AssignPrivileges();
                manager.CommitChanges();

            }
            catch (Exception e)
            {
                ErrorHandler.WriteError(e);
                return false;
            }

            return true;
        }
        public void AddVirtualDirectories()
        {


            foreach (VirtualDirectory directory in App.VirtualDirectories)
            {
                if (!Directory.Exists(directory.Path))
                {
                    ErrorHandler.WriteError(new DirectoryNotFoundException($"The following path was not found: {directory.Path}, Please make sure that the directory exists..."));
                    return;
                }
                manager.Sites[App.ApplicationName].Applications[App.ApplicationName].VirtualDirectories.Add(directory.VirtualName, directory.Path);
            }
        }
        /// <summary>
        /// Designates the proper privileges to the directory
        /// </summary>
        private void AssignPrivileges()
        {
            try
            {
                AddDirectorySecurity(App.Path, WindowsIdentity.GetCurrent().User.ToString().Split('\\').First() + "\\IIS_IUSRS", FileSystemRights.FullControl, AccessControlType.Allow);
                AddDirectorySecurity(App.Path, WindowsIdentity.GetCurrent().User.ToString(), FileSystemRights.FullControl, AccessControlType.Allow);
                AddDirectorySecurity(App.Path, "US\\US_SVC_EA_WEB_USR", FileSystemRights.FullControl, AccessControlType.Allow);

            }catch(Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }

        }

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the 
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);

        }



    }
}
