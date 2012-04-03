using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Abstracts the creation of the BrowserCommandExecutor to be used by HtmlPage.
    /// </summary>
    public interface IBrowserCommandExecutorFactory
    {
        /// <summary>
        /// CreateBrowserCommandExecutor
        /// </summary>
        IBrowserCommandExecutor CreateBrowserCommandExecutor(string applicationPath, HtmlPage page);
    }
}
