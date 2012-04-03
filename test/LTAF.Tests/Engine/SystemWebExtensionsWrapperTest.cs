using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.UI;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class SystemWebExtensionsWrapperTest
    {
        [TestMethod]
        public void Initialize()
        {
            MockAspNetService mockAspNetService = new MockAspNetService();
            SystemWebExtensionsWrapper swe = new SystemWebExtensionsWrapper(mockAspNetService);
            UnitTestAssert.IsNull(swe.RegisterStartupScriptMethodInfo);
            swe.Initialize(new Page());
            UnitTestAssert.IsNotNull(swe.RegisterStartupScriptMethodInfo);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeserializeJsonThrowsIfNoExtensionsReference()
        {
            MockAspNetService mockAspNetService = new MockAspNetService();
            SystemWebExtensionsWrapper swe = new SystemWebExtensionsWrapper(mockAspNetService);
            swe.SystemWebExtensionsAssembly = null;
            swe.DeserializeJson("{\"foo\":\"bar\"}");
        }


        [TestMethod]
        public void DeserializeJsonDictionary()
        {
            MockAspNetService mockAspNetService = new MockAspNetService();
            SystemWebExtensionsWrapper swe = new SystemWebExtensionsWrapper(mockAspNetService);
            swe.SystemWebExtensionsAssembly = Assembly.GetAssembly(typeof(ScriptManager));

            object o = swe.DeserializeJson("{\"foo\":\"bar\"}");
            UnitTestAssert.IsNotNull(o);
            Dictionary<string, object> dic = (Dictionary<string, object>)o;
            UnitTestAssert.AreEqual("bar", dic["foo"]);
        }

    }
}
