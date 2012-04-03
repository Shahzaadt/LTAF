using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;

namespace LTAF
{
    /// <summary>
    /// Describes a type that can execute BrowserCommands
    /// </summary>
    public interface IBrowserCommandExecutor
    {
        /// <summary>
        /// Execute a command on the target browser
        /// </summary>
        /// <param name="threadId">Id used to distinguish between multiple tests running at the same time</param>
        /// <param name="source">HtmlElement that initiated this command, null if none</param>
        /// <param name="command">Command to execute</param>
        /// <param name="secondsTimeout">Timeout in seconds that browser shoud wait for this command</param>
        /// <returns>BrowserInfo object that contains command results</returns>
        BrowserInfo ExecuteCommand(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout);
    }
}
