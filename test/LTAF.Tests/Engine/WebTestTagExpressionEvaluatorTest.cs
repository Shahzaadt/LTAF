using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Collections.Specialized;
using System.Reflection;


namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class WebTestTagExpressionEvaluatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(FormatException), "No tests corresponding to tag \"foo\" in expression \"foo\" were found!.")]
        public void EvaluateThrowsExceptionIfTestWithSuppliedTagIsNotFound()
        {
            WebTestTagExpressionEvaluator evaluator = new WebTestTagExpressionEvaluator();
            evaluator.Evaluate(
                "foo", 
                BrowserVersions.All, 
                new List<MethodInfo>(), 
                new Dictionary<System.Reflection.MethodInfo,BrowserVersions>(), 
                new Dictionary<string,List<System.Reflection.MethodInfo>>());
        }

        [TestMethod]
        public void EvaluateReturnsListWithASingleTestThatMatchesTheTag()
        {
            MethodInfo method = typeof(SampleWebTestClass).GetMethod("SampleWebTestMethod1");

            Dictionary<string, List<MethodInfo>> tags = new Dictionary<string, List<MethodInfo>>();
            tags.Add("foo", new List<MethodInfo>() { method });

            WebTestTagExpressionEvaluator evaluator = new WebTestTagExpressionEvaluator();
            List<MethodInfo> filtered = evaluator.Evaluate(
                "foo",
                BrowserVersions.All,
                 new List<MethodInfo>(), 
                new Dictionary<MethodInfo, BrowserVersions>(),
                tags);

            UnitTestAssert.AreEqual(1, filtered.Count);
            UnitTestAssert.AreSame(method, filtered[0]);
        }

        [TestMethod]
        public void EvaluateReturnsListWhenSeveralTagsAreUsed()
        {
            MethodInfo method1 = typeof(SampleWebTestClass).GetMethod("SampleWebTestMethod1");
            MethodInfo method2 = typeof(SampleWebTestClass).GetMethod("SampleWebTestMethod2");

            Dictionary<string, List<MethodInfo>> tags = new Dictionary<string, List<MethodInfo>>();
            tags.Add("foo", new List<MethodInfo>() { method1 });
            tags.Add("bar", new List<MethodInfo>() { method2 });

            WebTestTagExpressionEvaluator evaluator = new WebTestTagExpressionEvaluator();
            List<MethodInfo> filtered = evaluator.Evaluate(
                "foo@bar",
                BrowserVersions.All,
                 new List<MethodInfo>(), 
                new Dictionary<MethodInfo, BrowserVersions>(),
                tags);

            UnitTestAssert.AreEqual(2, filtered.Count);
        }

        [TestMethod]
        public void EvaluateIgnoresDuplicatedMethods()
        {
             MethodInfo method1 = typeof(SampleWebTestClass).GetMethod("SampleWebTestMethod1");
            MethodInfo method2 = typeof(SampleWebTestClass).GetMethod("SampleWebTestMethod1");

            Dictionary<string, List<MethodInfo>> tags = new Dictionary<string, List<MethodInfo>>();
            tags.Add("foo", new List<MethodInfo>() { method1 });
            tags.Add("bar", new List<MethodInfo>() { method2 });

            WebTestTagExpressionEvaluator evaluator = new WebTestTagExpressionEvaluator();
            List<MethodInfo> filtered = evaluator.Evaluate(
                "foo@bar",
                BrowserVersions.All,
                 new List<MethodInfo>(), 
                new Dictionary<MethodInfo, BrowserVersions>(),
                tags);

            UnitTestAssert.AreEqual(1, filtered.Count);
        }

    }
}
