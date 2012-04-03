using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// CommandExecutorFactory for LTAF tests executing within a Browser
    /// </summary>
    internal class BrowserCommandExecutorFactory: IBrowserCommandExecutorFactory
    {
        public IBrowserCommandExecutor CreateBrowserCommandExecutor(string applicationPath, HtmlPage page)
        {
            HtmlElementCollection.FindElementTimeout = 5;
            return new BrowserCommandExecutor();
        }
    }
}
