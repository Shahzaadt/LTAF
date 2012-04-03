using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;

namespace LTAF.UnitTests.Engine
{
    public class TestableTestcaseExecutor: TestcaseExecutor
    {
        public void RaiseRunStartedEvent(EventArgs e)
        {
            this.OnTestRunStarted(e);
        }

        public void RaiseRunFinishedEvent(TestRunFinishedEventArgs e)
        {
            this.OnTestRunFinished(e);
        }

        public void RaiseTestcaseExecutingEvent(TestcaseExecutionEventArgs e)
        {
            this.OnTestcaseExecuting(e);
        }

        public void RaiseTestcaseExecutedEvent(TestcaseExecutionEventArgs e)
        {
            this.OnTestcaseExecuted(e);
        }
    }
}
