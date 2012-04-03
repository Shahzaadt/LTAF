using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Engine
{
    internal delegate void BrowserQueueCreated(int threadId);

    internal class TestcaseExecutorTaskInfo
    {
        private BrowserQueueCreated _notifyBrowserQueueCreated;
        private ICollection<Testcase> _testcases;
        private IDictionary<string, string> _requestDetails;

        internal TestcaseExecutorTaskInfo(ICollection<Testcase> testcases, BrowserQueueCreated browserQueueCreatedCallback, IDictionary<string, string> requestDetails)
        {
            _testcases = testcases;
            _notifyBrowserQueueCreated = browserQueueCreatedCallback;
            _requestDetails = requestDetails;
        }

        internal ICollection<Testcase> Testcases
        {
            get { return _testcases; }
        }

        internal BrowserQueueCreated NotifyBrowserQueueCreated
        {
            get { return _notifyBrowserQueueCreated; }
        }

        internal IDictionary<string, string> RequestDetails
        {
            get { return _requestDetails; }
        }
    }
}
