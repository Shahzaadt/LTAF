using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;
using System.Web.UI;
using System.Collections.Specialized;

namespace LTAF.UnitTests
{
    public class MockAssertResultHandler: IAssertResultHandler
    {
        public string LastMessage { get; set; }


        #region IAssertResultHandler Members
        public void AssertFailed(string message)
        {
            this.LastMessage = message;
        }

        public void AssertPassed(string message)
        {
            this.LastMessage = message;
        }

        #endregion
    }
}
