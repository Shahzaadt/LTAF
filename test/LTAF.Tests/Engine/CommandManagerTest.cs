using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Threading;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class CommandManagerTest
    {

        [TestInitialize]
        public void SetUp()
        {
            CommandManager.DestroyAllBrowserQueues();
        }

        [TestMethod]
        public void VerifyTimeOut()
        {
            CommandManager.CreateBrowserQueue(1);
            BrowserCommand c = new BrowserCommand();
            UnitTestAssert.IsNull(CommandManager.ExecuteCommand(1, c, 0));
        }

        [TestMethod]
        public void VerifyGettingCommandAferCreation()
        {
            CommandManager.CreateBrowserQueue(1);
            UnitTestAssert.IsNull(CommandManager.SetResultAndGetNextCommand(1, null));
        }

        [TestMethod]
        public void VerifyCreatingQueuesWithSameId()
        {
            CommandManager.CreateBrowserQueue(1);
            CommandManager.CreateBrowserQueue(1);
            UnitTestAssert.IsNull(CommandManager.SetResultAndGetNextCommand(1, null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifySettingCommandWithOutCreation()
        {
            BrowserCommand c = new BrowserCommand();
            UnitTestAssert.IsNull(CommandManager.ExecuteCommand(1, c, 0));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyGettingCommandWithOutCreation()
        {
            UnitTestAssert.IsNull(CommandManager.SetResultAndGetNextCommand(1, null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyCommandFinishedWithOutCreation()
        {
            CommandManager.SetResultAndGetNextCommand(1, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VerifyDestroyingWithOutCreation()
        {
            CommandManager.DestroyBrowserQueue(1);
        }


        [TestMethod]
        public void VerifyBasicCommandSetGet()
        {
            CommandManager.CreateBrowserQueue(1);

            BrowserInfo browserInfo = new BrowserInfo();
            browserInfo.Data = "foobar";
            BrowserSimulator.WaitForOneCommand(1, browserInfo);

            BrowserCommand c = new BrowserCommand();
            UnitTestAssert.AreEqual("foobar", CommandManager.ExecuteCommand(1, c, 10).Data);
            
            browserInfo = CommandManager.GetBrowserInfo(1);
            UnitTestAssert.AreEqual("foobar", browserInfo.Data);
        }
    }
}
