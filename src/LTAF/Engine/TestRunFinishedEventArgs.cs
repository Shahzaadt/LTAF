using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Engine
{
    /// <summary>
    /// EventArgs provided when a test run completes
    /// </summary>
    public class TestRunFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Test cases that were run
        /// </summary>
        public ICollection<Testcase> TestCases
        {
            get { return _testCases ?? new List<Testcase>(); }
            set { _testCases = value; }
        }
        private ICollection<Testcase> _testCases;

        /// <summary>
        /// Details of the test run
        /// </summary>
        public IDictionary<string, string> Details
        {
            get { return _details ?? new Dictionary<string, string>(); }
            set
            {
                _details = value;
                _testLog = null;
            }
        }
        private IDictionary<string, string> _details;

        /// <summary>
        /// Log of the test run
        /// </summary>
        public string TestLog
        {
            get
            {
                if (_testLog == null && _details != null)
                {
                    _details.TryGetValue("TestLog", out _testLog);
                }
                return _testLog ?? "";
            }
        }
        private string _testLog;

        /// <summary>
        /// Create TestRunFinishedEventArgs
        /// </summary>
        public TestRunFinishedEventArgs()
            : this(null, null)
        {
        }

        /// <summary>
        /// Create TestRunFinishedEventArgs
        /// </summary>
        /// <param name="testCases">Test cases that were run</param>
        /// <param name="details">Details of the test run</param>
        public TestRunFinishedEventArgs(ICollection<Testcase> testCases, IDictionary<string, string> details)
        {
            _testCases = testCases;
            _details = details;
            _testLog = null;
        }
    }
}
