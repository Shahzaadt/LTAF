using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class TestcaseManagerTest
    {
        [TestMethod]
        public void TestClassesGetLoadsTests()
        {
            TestCaseManager tcm = new TestCaseManager();
            UnitTestAssert.AreEqual(1, tcm.TestClasses.Length);
        }

        [TestMethod]
        public void TestMethodsGetLoadsMethods()
        {
            TestCaseManager tcm = new TestCaseManager();
            UnitTestAssert.AreEqual(2, tcm.TestMethods.Length);
        }

        [TestMethod]
        public void LoadsTestsByTags()
        {
            TestCaseManager tcm = new TestCaseManager();
            UnitTestAssert.AreEqual(7, tcm.Tags.Count);
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("Tag1"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("Tag2"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("LTAF.UnitTests.SampleWebTestClass"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("LTAF.UnitTests.SampleWebTestClass.SampleWebTestMethod1"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("LTAF.UnitTests.SampleWebTestClass.SampleWebTestMethod2"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("SampleWebTestMethod1"));
            UnitTestAssert.IsTrue(tcm.Tags.ContainsKey("SampleWebTestMethod2"));
        }

        [TestMethod]
        public void PopulateTreeView()
        {
            TestCaseManager tcm = new TestCaseManager();
            TreeNode node = new TreeNode();
            tcm.PopulateTreeView(node);

            UnitTestAssert.AreEqual(1, node.ChildNodes.Count);
        }

        [TestMethod]
        public void GetSelectedTestcases()
        {
            TestCaseManager tcm = new TestCaseManager();
            TreeView tree = new TreeView();
            TreeNode node = new TreeNode();
            tree.Nodes.Add(node);

            tcm.PopulateTreeView(node);

            node.ChildNodes[0].ChildNodes[0].Checked = true;
            IList<Testcase> tests = tcm.GetSelectedTestCases(tree);

            UnitTestAssert.AreEqual(1, tests.Count);
        }

    }
}
