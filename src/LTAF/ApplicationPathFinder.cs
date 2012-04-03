using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Default ApplicationPathFinder, stores the application path calculated by the DriverPage
    /// </summary>
    internal class ApplicationPathFinder : IApplicationPathFinder
    {
        private string _applicationPath;

        public ApplicationPathFinder(string applicationPath)
        {
            _applicationPath = applicationPath;
        }


        public string GetApplicationPath()
        {
            return _applicationPath;
        }
    }
}
