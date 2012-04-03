using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;

namespace LTAF
{
    /// <summary>
    /// Default BrowserCommandExecutor, delegates all calls to the CommandManager
    /// </summary>
    public class BrowserCommandExecutor : IBrowserCommandExecutor
    {
        private const int NAVIGATE_WAIT_TO_LOAD_MILLISECONDS = 500;

        /// <summary>
        /// Execute a command on the browser
        /// </summary>
        /// <param name="threadId">Id used to distinguish between multiple tests running at the same time</param>
        /// <param name="source">HtmlElement that initiated this command, null if none</param>
        /// <param name="command">Command to execute</param>
        /// <param name="secondsTimeout">Timeout in seconds that executor shoud wait for this command</param>
        /// <returns>BrowserInfo object that contains command results</returns>
        public BrowserInfo ExecuteCommand(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            var browserInfo = CommandManager.ExecuteCommand(System.Threading.Thread.CurrentThread.ManagedThreadId, command, secondsTimeout);

            if (command.Handler.ClientFunctionName == BrowserCommand.FunctionNames.NavigateToUrl)
            {
                System.Threading.Thread.Sleep(NAVIGATE_WAIT_TO_LOAD_MILLISECONDS);
            }

            return browserInfo;
        }
    }
}
