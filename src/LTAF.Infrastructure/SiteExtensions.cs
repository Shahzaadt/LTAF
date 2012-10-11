using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Web.Administration;

namespace LTAF.Infrastructure
{
    public static class SiteExtensions
    {
        public static string GetHttpVirtualPath(this Site site, string hostName)
        {
            return GetVirtualPath(site, "http", hostName);
        }

        public static string GetHttpVirtualPath(this Site site)
        {
            return GetVirtualPath(site, "http", "");
        }

        public static string GetHttpsVirtualPath(this Site site, string hostName)
        {
            return GetVirtualPath(site, "https", hostName);
        }

        public static string GetHttpsVirtualPath(this Site site)
        {
            return GetVirtualPath(site, "https", "");
        }

        public static string GetUniqueApplicaionName(this Site site, string appName)
        {
            appName = appName.Trim('/');

            int counter = 0;     
            string uniquePath = appName;
            while (true)
            {
                var apps = site.Applications.Where(
                    a => string.Compare(a.Path.Trim('/'), uniquePath, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (apps.Count() <= 0)
                {
                    break;
                }

                uniquePath = appName + "_" + (++counter).ToString();
            }

            return uniquePath;
        }

        internal static string GetVirtualPath(Site site, string protocol, string hostName)
        {
            Binding binding = null;
            var bindings = site.Bindings.Where(b => b.Protocol == protocol);
            if (bindings.Count() >= 1)
            {
                binding = bindings.Single();
            }
            else
            {
                throw new Exception(string.Format("Binding for protocol '{0}' is not defined for the website '{1}'.", protocol, site.Name));
            }

            string host = "localhost";
            if (!string.IsNullOrEmpty(hostName))
            {
                host = hostName;
            } else if (!string.IsNullOrEmpty(binding.Host))
            {
                host = binding.Host;
            }

            if (binding.EndPoint.Port == 80)
            {
                return string.Format("{0}://{1}", protocol, host);
            }
            else
            {
                return string.Format("{0}://{1}:{2}", protocol, host, binding.EndPoint.Port);
            }
        }
    }
}
