using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace LTAF.Engine
{
    /// <summary>
    /// Testcase
    /// </summary>
    public class Testcase
    {
        private Type _classType;
        private string _methodName;

        /// <summary>TestcaseExecuting</summary>
        public event EventHandler<TestcaseExecutionEventArgs> TestcaseExecuting;
        /// <summary>TestcaseExecuted</summary>
        public event EventHandler<TestcaseExecutionEventArgs> TestcaseExecuted;

        /// <summary>
        /// ctor
        /// </summary>
        public Testcase(Type classType, string methodName)
        {
            _classType = classType;
            _methodName = methodName;
        }

        /// <summary>
        /// Execute
        /// </summary>
        public void Execute()
        {
            object webTestObject = Activator.CreateInstance(_classType);

            TestcaseExecutionEventArgs args = new TestcaseExecutionEventArgs(_classType.FullName, _methodName, true);
            OnTestcaseExecuting(args);

            try
            {
                _classType.InvokeMember(_methodName, BindingFlags.InvokeMethod, null, webTestObject, new object[] { });
            }
            catch (TargetInvocationException e)
            {
                //now what do I do?
                args.Passed = false;
                args.Exception = e.InnerException;
            }
            finally
            {
                OnTestcaseExecuted(args);
                //If the class implements IDisposable call the dispose method
                if (webTestObject is IDisposable)
                {
                    ((IDisposable)webTestObject).Dispose();
                }
            }
        }

        private void OnTestcaseExecuting(TestcaseExecutionEventArgs e)
        {
            if (this.TestcaseExecuting != null)
            {
                this.TestcaseExecuting(this, e);
            }
        }

        private void OnTestcaseExecuted(TestcaseExecutionEventArgs e)
        {
            if (e != null && e.Exception != null && e.Exception.GetType() == typeof(TimeoutException))
            {
                throw e.Exception;
            }

            if (this.TestcaseExecuted != null)
            {
                this.TestcaseExecuted(this, e);
            }
        }
    }
}
