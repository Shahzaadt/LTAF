using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace LTAF.UnitTests
{
    public static class ExceptionAssert
    {
        public static void ThrowsInvalidOperation(Action action, string expectedMessage)
        {
            Throws<InvalidOperationException>(action, expectedMessage);
        }

        public static void ThrowsElementNotFound(Action action)
        {
            Throws<ElementNotFoundException>(action);
        }

        public static void Throws<TException>(Action action) where TException : Exception
        {
            Exception ex = CaptureException(action);
            MSAssert.IsNotNull(ex, "The expected exception was not thrown");
            MSAssert.IsInstanceOfType(ex, typeof(TException), "The exception thrown was not of the expected type");
        }

        public static void Throws<TException>(Action action, string expectedMessage) where TException : Exception
        {
            Exception ex = CaptureException(action);
            MSAssert.IsNotNull(ex, "The expected exception was not thrown");
            MSAssert.IsInstanceOfType(ex, typeof(TException), "The exception thrown was not of the expected type");
            MSAssert.AreEqual(expectedMessage, ex.Message, String.Format("Exception message was not as expected"));
        }

        private static Exception CaptureException(Action action)
        {
            Exception ex = null;
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                ex = e;
            }
            return ex;
        }
    }
}
