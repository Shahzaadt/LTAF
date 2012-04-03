using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF;
using LTAF.Engine;

namespace LTAF.UnitTests
{
    class MockCommandExecutor : IBrowserCommandExecutor
    {
        private BrowserInfo _browserInfo;
        private List<BrowserCommand> _executedCommands;
        public event EventHandler ExecutingCommand;

        public BrowserCommand[] ExecutedCommands
        {
            get
            {
                return _executedCommands.ToArray();
            }
        }

        public MockCommandExecutor()
        {
            this._executedCommands = new List<BrowserCommand>();
            this.SetBrowserInfo(new BrowserInfo());
        }

        public BrowserInfo ExecuteCommand(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            OnExecutingCommand(new EventArgs());
            _executedCommands.Add(command);
            return _browserInfo;
        }

        protected void OnExecutingCommand(EventArgs e)
        {
            if (ExecutingCommand != null)
            {
                ExecutingCommand(this, e);
            }
        }

        public void SetBrowserInfo(BrowserInfo browserInfo)
        {
            _browserInfo = browserInfo;
        }
    }
}
