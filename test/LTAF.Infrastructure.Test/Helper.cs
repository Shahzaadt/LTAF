using System;
using System.IO;
using System.Reflection;
using Microsoft.Web.Administration;

namespace LTAF.Infrastructure.Test
{
    static class Helper
    {
        public static string Randomize(string val)
        {
            Random r1 = new Random((int)DateTime.Now.Ticks/2);
            Random r2 = new Random(r1.Next());
            val = val + "_" + r1.Next().ToString() + "_" + r2.Next().ToString();

            return val;
        }

        private static string _applicationHostTemplate = null;
        public static string AplicationHostTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationHostTemplate))
                {
                    var assembly = Assembly.GetExecutingAssembly();

                    var list = assembly.GetManifestResourceNames();
                    using (var stream = assembly.GetManifestResourceStream("LTAF.Infrastructure.Test.applicationhost_template.config"))
                    using (var reader = new StreamReader(stream))
                    {
                        _applicationHostTemplate = Path.Combine(Path.GetDirectoryName(assembly.Location), "applicationhost_template.config");
                        File.WriteAllText(_applicationHostTemplate, reader.ReadToEnd());
                    }
                }

                return _applicationHostTemplate;
            }
        }

        public static ServerManager ServerManager
        {
            get
            {
                   return new ServerManager(Helper.AplicationHostTemplate);
            }
        }
    }
}
