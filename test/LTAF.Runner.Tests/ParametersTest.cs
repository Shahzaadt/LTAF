using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Runner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.Runner.Tests
{
    [TestClass]
    public class ParametersTest
    {
        public string[] GenerateCommandLineArgs(string parameters)
        {
            return parameters.Split(' ');
        }


        [TestMethod]
        public void Parse_Timeout()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/Timeout:40"));
            Assert.AreEqual(2400000, p.ExecutionTimeout);

            p.Parse(GenerateCommandLineArgs(@"/TIMEOUT:50"));
            Assert.AreEqual(3000000, p.ExecutionTimeout);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Timeout_NonIntValue()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/Timeout:foo"));
        }

        [TestMethod]
		public void Parse_TagNameCasing()
		{
			RunParameters p = new RunParameters();
			p.Parse(GenerateCommandLineArgs(@"/TAG:Foobar"));
			Assert.AreEqual("Foobar", p.TagName);
		}

        [TestMethod]
        public void Parse_TagName()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/tag:Foobar"));
            Assert.AreEqual("Foobar", p.TagName);
        }

        [TestMethod]
        public void Parse_BrowserShortLongCasing()
        {
            //IE
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/browser:IE"));
            Assert.IsInstanceOfType(p.Browser, typeof(IEBrowser));

            p.Parse(GenerateCommandLineArgs(@"/browser:INTERNETEXPLORER"));
            Assert.IsInstanceOfType(p.Browser, typeof(IEBrowser));

            p.Parse(GenerateCommandLineArgs(@"/browser:internetexplorer"));
            Assert.IsInstanceOfType(p.Browser, typeof(IEBrowser));


            //FIREFOX
            p.Parse(GenerateCommandLineArgs(@"/browser:FF"));
            Assert.IsInstanceOfType(p.Browser, typeof(FireFoxBrowser));

            p.Parse(GenerateCommandLineArgs(@"/browser:FIREFOX"));
            Assert.IsInstanceOfType(p.Browser, typeof(FireFoxBrowser));

            p.Parse(GenerateCommandLineArgs(@"/browser:firefox"));
            Assert.IsInstanceOfType(p.Browser, typeof(FireFoxBrowser));
        }

        [TestMethod]
        public void Parse_Help()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/?"));
            Assert.IsTrue(p.PrintHelp);
        }

        [TestMethod]
        public void Parse_BrowserAndPath()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/browser:ie /path:D:\DD\Tests\myWebsites"));
			Assert.AreEqual(@"D:\DD\Tests\myWebsites", p.WebSitePath);
            Assert.IsInstanceOfType(p.Browser, typeof(IEBrowser));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_UnknownParameter()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/z:D:\inetpub\wwwroot\foo"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_InvalidParameter()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/u:foo"));
        }

        [TestMethod]
        public void Parse_1Param()
        {
            RunParameters p = new RunParameters();
            p.Parse(GenerateCommandLineArgs(@"/path:D:\DD\Tests\myWebsites"));
			Assert.AreEqual(@"D:\DD\Tests\myWebsites", p.WebSitePath);
        }

        [TestMethod]
        public void DefaultValues()
        {
            RunParameters p = new RunParameters();
            Assert.IsInstanceOfType(p.Browser, typeof(IEBrowser));
            Assert.AreEqual(1800000, p.ExecutionTimeout);
            Assert.IsNull(p.WebSitePath);
            Assert.IsNull(p.TagName);
            Assert.IsFalse(p.PrintHelp);
        }
    }
}
