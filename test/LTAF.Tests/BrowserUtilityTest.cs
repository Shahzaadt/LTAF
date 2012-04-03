using System;
using System.Collections.Generic;
using System.Text;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests
{
    [TestClass]
    public class BrowserUtilityTest
    {
        [TestMethod]
        public void GetBrowserIE6()
        {
            UnitTestAssert.AreEqual(BrowserVersions.InternetExplorer6, BrowserUtility.GetBrowser("IE", 6));
        }

        [TestMethod]
        public void GetBrowserIE7()
        {
            UnitTestAssert.AreEqual(BrowserVersions.InternetExplorer7, BrowserUtility.GetBrowser("IE", 7));
        }

        [TestMethod]
        public void GetBrowserIE8()
        {
            UnitTestAssert.AreEqual(BrowserVersions.InternetExplorer8, BrowserUtility.GetBrowser("IE", 8));
        }

        [TestMethod]
        public void GetBrowserIEFutureVersion()
        {
            UnitTestAssert.AreEqual(BrowserVersions.InternetExplorer, BrowserUtility.GetBrowser("IE", 9));
        }

        [TestMethod]
        public void GetBrowserFireFox1()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Firefox1, BrowserUtility.GetBrowser("FireFox", 1));
        }

        [TestMethod]
        public void GetBrowserFireFox2()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Firefox2, BrowserUtility.GetBrowser("FireFox", 2));
        }

        [TestMethod]
        public void GetBrowserFireFox3()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Firefox3, BrowserUtility.GetBrowser("FireFox", 3));
        }

        [TestMethod]
        public void GetBrowserFireFoxFutureVersion()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Firefox, BrowserUtility.GetBrowser("FireFox", 5));
        }

        [TestMethod]
        public void GetBrowserOpera9()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Opera9, BrowserUtility.GetBrowser("Opera", 9));
        }

        [TestMethod]
        public void GetBrowserOpera10()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Opera10, BrowserUtility.GetBrowser("Opera", 10));
        }

        [TestMethod]
        public void GetBrowserOperaFutureVersion()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Opera, BrowserUtility.GetBrowser("Opera", 5));
        }

        [TestMethod]
        public void GetBrowserSafariMac2()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Safari2, BrowserUtility.GetBrowser("applemac-safari", 2));
        }

        [TestMethod]
        public void GetBrowserSafariMac3Mac()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Safari3Mac, BrowserUtility.GetBrowser("applemac-safari", 3));
        }

        [TestMethod]
        public void GetBrowserSafariMac3Windows()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Safari3Windows, BrowserUtility.GetBrowser("applemac-safari", 5));
        }

        [TestMethod]
        public void GetBrowserSafariFutureVersion()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Safari, BrowserUtility.GetBrowser("applemac-safari", 10));
        }

        [TestMethod]
        public void GetBrowserSafari_AspNet4()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Safari, BrowserUtility.GetBrowser("safari", 10));
        }

        [TestMethod]
        public void GetBrowserChrome()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Chrome, BrowserUtility.GetBrowser("chrome", 2));
        }

        [TestMethod]
        public void GetBrowserUnknown()
        {
            UnitTestAssert.AreEqual(BrowserVersions.Unknown, BrowserUtility.GetBrowser("foo", 123));
        }
    }
}
