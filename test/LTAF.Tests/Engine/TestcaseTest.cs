using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class TestcaseTest
    {
        [TestMethod]
        public void TestcaseExecuteCreatesAndDisposesInstancesWhenIDisposable()
        {
            try
            {
                Testcase test = new Testcase(typeof(DisposableStubTestcase), "FakeTest");
                test.Execute();
                UnitTestAssert.IsTrue(DisposableStubTestcase.ctorCounter == 1);
                UnitTestAssert.IsTrue(DisposableStubTestcase.methodCounter == 1);
                UnitTestAssert.IsTrue(DisposableStubTestcase.disposeCounter == 1);
            }
            finally
            {
                DisposableStubTestcase.Reset();
            }
        }

        [TestMethod]
        public void TestcaseExecuteCreatesAndDisposesInstancesWhenNotIDisposable()
        {
            try
            {
                Testcase test = new Testcase(typeof(StubTestcase), "FakeTest");
                test.Execute();
                UnitTestAssert.IsTrue(StubTestcase.ctorCounter == 1);
                UnitTestAssert.IsTrue(StubTestcase.methodCounter == 1);
                UnitTestAssert.IsTrue(StubTestcase.disposeCounter == 0);
            }
            finally
            {
                StubTestcase.Reset();
            }
        }
    }

    internal class DisposableStubTestcase : IDisposable
    {
        public static int ctorCounter;
        public static int disposeCounter;
        public static int methodCounter;

        public DisposableStubTestcase()
        {
            ctorCounter++;
        }

        public void FakeTest()
        {
            methodCounter++;
        }

        public void Dispose()
        {
            disposeCounter++;
        }

        public static void Reset()
        {
            ctorCounter = 0;
            methodCounter = 0;
            disposeCounter = 0;
        }
    
    }


    internal class StubTestcase
    {
        public static int ctorCounter;
        public static int disposeCounter;
        public static int methodCounter;

        public StubTestcase()
        {
            ctorCounter++;
        }

        public void FakeTest()
        {
            methodCounter++;
        }

        public void Dispose()
        {
            disposeCounter++;
        }

        public static void Reset()
        {
            ctorCounter = 0;
            methodCounter = 0;
            disposeCounter = 0;
        }

    }

}
