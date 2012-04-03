using System;
using System.Collections.Generic;
using System.Text;
using LTAF.Engine;
using System.Threading;

namespace LTAF.UnitTests
{
    public class BrowserSimulator
    {
        private static BrowserInfo _browserInfo;
        private static int _threadId;
        public static void WaitForOneCommand(int threadId, BrowserInfo browserInfoToReturn)
        {
            _browserInfo = browserInfoToReturn;
            _threadId = threadId;

            Thread t = new Thread(new ThreadStart(SimulateBrowser));
            t.Start();

            Thread.Sleep(500);
        }

        private static void SimulateBrowser()
        {
            BrowserCommand c;
            int attempts = 0;
            do
            {
                c = CommandManager.SetResultAndGetNextCommand(_threadId, null);
                Thread.Sleep(250);
                attempts++;
            } while (c == null && attempts <= 40);

            CommandManager.SetResultAndGetNextCommand(_threadId, _browserInfo);
        }
    }
}
