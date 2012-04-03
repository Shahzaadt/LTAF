namespace Microsoft.Internal.Test {
    using System;
    using System.DirectoryServices;
    using System.IO;
    using System.Net;
    using System.Threading;

    public static class IISHelper {
        private const int webSiteNum = 1;

        public static void CreateVDir(string virtualDirectoryName, string path, string serverName) {
            DirectoryEntry iisSchema;
            DirectoryEntry iisAdmin;
            DirectoryEntry vDir;
            bool iisUnderNT;

            // Determine version of IIS
            iisSchema = new DirectoryEntry("IIS://" + serverName + "/Schema/AppIsolated");
            if (iisSchema.Properties["Syntax"].Value.ToString().ToUpper() == "BOOLEAN")
                iisUnderNT = true;
            else
                iisUnderNT = false;
            iisSchema.Dispose();

            // Get the admin object
            iisAdmin = new DirectoryEntry("IIS://" + serverName + "/W3SVC/" + webSiteNum + "/Root");

            // If we're not creating a root directory
            // If the virtual directory already exists then delete it

            foreach (DirectoryEntry v in iisAdmin.Children) {
                if (v.Name == virtualDirectoryName) {
                    // Delete the specified virtual directory if it already exists
                    iisAdmin.Invoke("Delete", new string[] { v.SchemaClassName, virtualDirectoryName });
                    iisAdmin.CommitChanges();
                }
            }

            // Create the virtual directory
            vDir = iisAdmin.Children.Add(virtualDirectoryName, "IIsWebVirtualDir");

            // Setup the vDir
            vDir.Properties["AccessRead"][0] = true;
            vDir.Properties["AccessExecute"][0] = false;
            vDir.Properties["AccessWrite"][0] = false;
            vDir.Properties["AccessScript"][0] = true;
			// Below, we need to set Integrated Windows Auth to true so that it enables debugging 
			// on the website as well as allows WindowsAuthentication when set inside a 
			// Web Application 
            vDir.Properties["AuthNTLM"][0] = true; 
            vDir.Properties["EnableDefaultDoc"][0] = true;
            vDir.Properties["EnableDirBrowsing"][0] = true;
            vDir.Properties["DefaultDoc"][0] = true;
            vDir.Properties["Path"][0] = path;

            // NT doesn't support this property
            if (!iisUnderNT) {
                vDir.Properties["AspEnableParentPaths"][0] = true;
            }

            // Set the changes
            vDir.CommitChanges();

            // Make it a web application
            if (iisUnderNT) {
                vDir.Invoke("AppCreate", false);
            }
            else {
                vDir.Invoke("AppCreate", 1);
            }

            WaitForVDir(virtualDirectoryName, serverName);
        }

        private static void WaitForVDir(string virtualDirectoryName, string serverName) {
            // On Windows Server 2008, the application may be unavailable for several seconds after
            // it is created, so we poll the application until it is ready (for a maximum 10 seconds).
            for (int iter = 0; iter < 100; iter++) {
                try {
                    // Poll by making requests for the directory listing at the application root.
                    // HttpWebRequest.GetResponse() will throw if the server returns an error.
                    string appRoot = "http://" + serverName + "/" + virtualDirectoryName + "/";
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(appRoot);

                    // Must dispose HttpWebResponse to avoid leaking resources.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    }
                }
                catch {
                    Thread.Sleep(100);
                }
            }
        }

        public static void DeleteVDir(string virtualDirectoryName, string serverName) {
            // Get the admin object
            DirectoryEntry iisAdmin = new DirectoryEntry("IIS://" + serverName + "/W3SVC/" + webSiteNum + "/Root");

            foreach (DirectoryEntry v in iisAdmin.Children) {
                if (v.Name == virtualDirectoryName) {
                    // Delete the specified virtual directory if it already exists
                    iisAdmin.Invoke("Delete", new string[] { v.SchemaClassName, virtualDirectoryName });
                    iisAdmin.CommitChanges();
                }
            }
        }
    }
}
