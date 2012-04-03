using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;


namespace LTAF
{
    /// <summary>
    /// Class that can locate services used by core types. Used for extensibility
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// The IApplicationPathFinder used by HtmlPage to calculate navigate Url
        /// </summary>
        public static IApplicationPathFinder ApplicationPathFinder
        {
            get;
            set;
        }

        /// <summary>
        /// The IBrowserCommandExecutorFactory used by the HtmlPage to construct a BrowserCommandExecutor
        /// </summary>
        public static IBrowserCommandExecutorFactory BrowserCommandExecutorFactory
        {
            get;
            set;
        }

        /// <summary>
        /// An IAssertResultHandler that is called whenever an Assert failes or succeeds
        /// </summary>
        public static IAssertResultHandler AssertResultHandler
        {
            get;
            set;
        }

        /// <summary>
        /// The IWebResourceReader used by the WebResourceReader class to read resources from assemblies
        /// </summary>
        public static IWebResourceReader WebResourceReader
        {
            get;
            set;
        }
    }
}
