using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Collections.Specialized;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class QueryStringParemetersTest
    {
        [TestMethod]
        public void DefaultValues()
        {
            QueryStringParameters p = new QueryStringParameters();
            UnitTestAssert.AreEqual(WebTestLogDetail.Default, p.LogDetail);
            UnitTestAssert.IsNull(p.Tag);
            UnitTestAssert.IsFalse(p.Filter);
            UnitTestAssert.IsFalse(p.Run);
            UnitTestAssert.IsFalse(p.SkipFail);
            UnitTestAssert.IsFalse(p.WriteLog);
            UnitTestAssert.IsTrue(p.ShowConsole);
        }

        [TestMethod]
        public void LoadFromQueryString_ShowConsoleTrueIfNotPresent()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("foo", "bar");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.ShowConsole);
        }

        [TestMethod]
        public void LoadFromQueryString_ShowConsole()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("console", "false");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsFalse(p.ShowConsole);
        }

        [TestMethod]
        public void LoadFromQueryStringTag()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("tag", "foo");
            p.LoadFromQueryString(col);
            UnitTestAssert.AreEqual("foo", p.Tag);
        }

        [TestMethod]
        public void LoadFromQueryStringFilter()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("filter", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.Filter);
        }

        [TestMethod]
        public void LoadFromQueryStringSkipFailTrueAndNoTag()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("skipfail", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.SkipFail);
            UnitTestAssert.AreEqual("!Fail", p.Tag);
        }


        [TestMethod]
        public void LoadFromQueryStringSkipFailTrueAndTag()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("skipfail", "true");
            col.Add("tag", "foo");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.SkipFail);
            UnitTestAssert.AreEqual("(foo)-Fail", p.Tag);
        }

        [TestMethod]
        public void LoadFromQueryStringWriteLogTrue()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("log", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.WriteLog);
        }

        [TestMethod]
        public void LoadFromQueryStringRunTrue()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("run", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsTrue(p.Run);
        }

        [TestMethod]
        public void LoadFromQueryStringRunFalse()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("run", "false");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsFalse(p.Run);
        }

        [TestMethod]
        public void LoadFromQueryStringRunInvalid()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("run", "blah");
            p.LoadFromQueryString(col);
            UnitTestAssert.IsFalse(p.Run);
        }

        [TestMethod]
        public void LoadFromQueryStringLogDetailConcise()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("concise", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.AreEqual(WebTestLogDetail.Concise, p.LogDetail);
        }

        [TestMethod]
        public void LoadFromQueryStringLogVerbose()
        {
            QueryStringParameters p = new QueryStringParameters();
            NameValueCollection col = new NameValueCollection();
            col.Add("verbose", "true");
            p.LoadFromQueryString(col);
            UnitTestAssert.AreEqual(WebTestLogDetail.Verbose, p.LogDetail);
        }
    }
}
