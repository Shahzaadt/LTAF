using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Engine
{
    /// <summary>
    /// Contains information about the execution of a test.
    /// </summary>
    public class TestcaseExecutionEventArgs : EventArgs
    {
        private string _webTestClassName;
        private string _webTestMethodName;
        private bool _passed;
        private Exception _exception;

        /// <summary>
        /// TestcaseExecutionEventArgs
        /// </summary>
        public TestcaseExecutionEventArgs(string webTestClassName, string webTestMethodName, bool passed)
        {
            _webTestClassName = webTestClassName;
            _webTestMethodName = webTestMethodName;
            _passed = passed;
        }

        /// <summary>
        /// The name of the testcase
        /// </summary>
        public string WebTestName
        {
            get { return _webTestClassName + "." + _webTestMethodName; }
        }

        /// <summary>
        /// Whether the test passed
        /// </summary>
        public bool Passed
        {
            get { return _passed; }
            set { _passed = value; }
        }

        /// <summary>
        /// Exception captured during testcase execution.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}
