using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Emulators
{
    /// <summary>
    /// CommandExecutorFactory to use when LTAF executes with a BrowserEmulator
    /// </summary>
    internal class EmulatedBrowserCommandExecutorFactory: IBrowserCommandExecutorFactory
    {
        public IBrowserCommandExecutor CreateBrowserCommandExecutor(string applicationPath, HtmlPage page)
        {
            var browserEmulator = new BrowserEmulator(applicationPath);

            // set time out to zero, to limit find elemenet attempts to 1
            HtmlElementCollection.FindElementTimeout = 0;

            return browserEmulator.CreateCommandExecutor();
        }
    }
}
