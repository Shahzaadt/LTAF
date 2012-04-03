using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests
{
    [TestClass]
    public class WebResourceReaderTest
    {

        [TestMethod]
        public void TriggerEventIfAttached()
        {
            ServiceLocator.WebResourceReader = new MockResourceReader();
            MockWebApplication app = new MockWebApplication();
            WebResourceReader reader = new WebResourceReader(app);

            string result = null;
            try
            {
                result = reader.GetString("System.Web.Extensions", "", "");
                Assert.AreEqual("my dummy string", result);
            }
            finally
            {
                ServiceLocator.WebResourceReader = null;
            }
        }


        [TestMethod]
        public void LocateAssemblyWithVersion()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                };
            app.SetReferencedAssemblies(asm);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions, Version=3.6.0.0", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
   
        }

        [TestMethod]
        public void LocateAssemblyByFullNameWithoutProcessor()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                };
            app.SetReferencedAssemblies(asm);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
        }


        [TestMethod]
        public void LocateAssemblyByFullNameWithProcessor()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL",
                "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL",
                };
            app.SetReferencedAssemblies(asm);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL", app.AssemblyName);
        }

        [TestMethod]
        public void LocateAssemblyByPartialName()
        {
            MockWebApplication app = new MockWebApplication();
            app.SetReferencedAssemblies(new List<string> { "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" });
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
        }

        [TestMethod]
        public void TrimDllFromAssemblyName()
        {
            MockWebApplication app = new MockWebApplication();
            app.SetReferencedAssemblies(new List<string> { "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" });
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions.dll", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
        }

        [TestMethod]
        public void LocateAssemblyByPartialNameSubMatches()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Web, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                };
            app.SetReferencedAssemblies(asm);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web", "", "");

            Assert.AreEqual("System.Web, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
        }

        [TestMethod]
        public void LocateAssemblyByPartialNameMultipleMatches()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                "System.Web.Extensions, Version=3.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                };
            app.SetReferencedAssemblies(asm);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");

            Assert.AreEqual("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", app.AssemblyName);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void LocateAssemblyByPartialNameNotFound()
        {
            MockWebApplication app = new MockWebApplication();
            app.SetReferencedAssemblies(new List<string>());
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LocateAssemblyByPartialNameThrowsException()
        {
            MockWebApplication app = new MockWebApplication();
            app.ThrowExceptionInGetAssmblies = true;
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void GetStringThrowsException()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                };
            app.SetReferencedAssemblies(asm);
            app.ThrowExceptionOnGetResourceString = true;
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void ThrowErrorIfGetStringReturnsNull()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                };
            app.SetReferencedAssemblies(asm);
            app.SetReturnValueForGetString(null);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void ThrowErrorIfGetStringReturnsEmptyString()
        {
            MockWebApplication app = new MockWebApplication();
            List<string> asm = new List<string>{
                "System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
                };
            app.SetReferencedAssemblies(asm);
            app.SetReturnValueForGetString(String.Empty);
            WebResourceReader reader = new WebResourceReader(app);
            reader.GetString("System.Web.Extensions", "", "");
        }


        [TestMethod]
        public void WhenGetStringByAssembly_IfAssemblyIsNull_ShouldThrow()
        {
            // Act
            MockWebApplication app = new MockWebApplication();            

            WebResourceReader reader = new WebResourceReader(app);

            // Assert
            ExceptionAssert.Throws<WebTestException>(
                () => reader.GetString((Assembly)null, "", ""),
                "Failed to load resource string. Assembly provided was null.");
        }

        [TestMethod]
        public void WhenGetStringByAssembly_IfInnerException_ShouldThrow()
        {
            // Act
            MockWebApplication app = new MockWebApplication();
            app.ThrowExceptionOnGetResourceString = true;

            WebResourceReader reader = new WebResourceReader(app);

            // Assert
            ExceptionAssert.Throws<WebTestException>(
                () => reader.GetString((Assembly)Assembly.GetExecutingAssembly(), "", ""));
        }

        [TestMethod]
        public void WhenGetStringByAssembly_IfResourceNotFound_ShouldThrow()
        {
            // Act
            MockWebApplication app = new MockWebApplication();
            app.SetReturnValueForGetString("");
            WebResourceReader reader = new WebResourceReader(app);

            // Assert
            ExceptionAssert.Throws<WebTestException>(
                () => reader.GetString((Assembly)Assembly.GetExecutingAssembly(), "", ""));
        }

        [TestMethod]
        public void WhenGetStringByAssembly_IfResourceFound_ShouldReturnResource()
        {
            // Act
            MockWebApplication app = new MockWebApplication();
            
            WebResourceReader reader = new WebResourceReader(app);

            // Assert
            Assert.AreEqual("dummyString", reader.GetString((Assembly)Assembly.GetExecutingAssembly(), "", ""));
        }
    }
}
