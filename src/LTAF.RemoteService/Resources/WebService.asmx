<%@ WebService Language="C#" Class="Microsoft.Web.Testing.WebService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using System.Collections;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Collections.Generic;
using LTAF.Engine;
using System.Text;

namespace Microsoft.Web.Testing
{
    /// <summary>
    /// WebService deployed to the test site that acts as the broker between the testcase
    /// and the browser
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    [System.Web.Script.Services.GenerateScriptType(typeof(LTAF.PopupAction))]
    public class WebService : System.Web.Services.WebService
    {
        /// <summary>The thread id for the ajax browser</summary>
        private const int BROWSER_QUEUE_ID = 1;
    
        /// <summary>
        /// Service consumed by browser to get the next command to execute
        /// </summary>
        /// <returns>The next command to execute, null if none exists</returns>
        [WebMethod]
        public BrowserCommand GetCommand(BrowserInfo resultOfLastCommand)
        {
            if(CommandManager.GetEntry(BROWSER_QUEUE_ID, false) == null)
            {
                CommandManager.CreateBrowserQueue(BROWSER_QUEUE_ID);
            }
            return CommandManager.SetResultAndGetNextCommand(BROWSER_QUEUE_ID, resultOfLastCommand);
        }

        /// <summary>
        /// Service consumed by Nexus to send commands to execute and waits until a BrowserInfo is returned by browser
        /// </summary>
        /// <param name="command">The command object to be sent to browser</param>
        /// <param name="secondsTimeout">The timeout that this method waits for the browser to call CommandFinished</param>
        /// <returns>A BrowserInfo object with info collected by browser during command's execution</returns>
        [WebMethod]
        public BrowserInfo ExecuteCommand(BrowserCommand command, int secondsTimeout)
        {
            if(CommandManager.GetEntry(BROWSER_QUEUE_ID, false) == null)
            {
                CommandManager.CreateBrowserQueue(BROWSER_QUEUE_ID);
            }
            return CommandManager.ExecuteCommand(BROWSER_QUEUE_ID, command, secondsTimeout);
        }

        /// <summary>
        /// Service consumed by Nexus to initialize the CommandManager browser queue
        /// </summary>
        [WebMethod]
        public void CreateBrowserQueue()
        {
            CommandManager.CreateBrowserQueue(BROWSER_QUEUE_ID);
        }
    }
}