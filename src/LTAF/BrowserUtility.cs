using System;
using System.Web;
using System.Web.UI;

namespace LTAF
{
    /// <summary>
    /// Utility to determine the current page's browser
    /// </summary>
    internal static class BrowserUtility
    {
        /// <summary>
        /// Gets the browser version for the current user agent
        /// </summary>
        /// <param name="userAgent">HttpBrowserCapabilities.Browser</param>
        /// <param name="majorVersion">HttpBrowserCapabilities.MajorVersion</param>
        /// <returns>Enum representing the Browser version</returns>
        internal static BrowserVersions GetBrowser(string userAgent, int majorVersion)
        {
            switch (userAgent.ToUpperInvariant())
            {
                case "IE":
                    switch (majorVersion)
                    {
                        case 6:
                            return BrowserVersions.InternetExplorer6;
                        case 7:
                            return BrowserVersions.InternetExplorer7;
                        case 8:
                            return BrowserVersions.InternetExplorer8;
                        default:
                            return BrowserVersions.InternetExplorer;
                    }
                case "FIREFOX":
                    switch (majorVersion)
                    {
                        case 1:
                            return BrowserVersions.Firefox1;
                        case 2:
                            return BrowserVersions.Firefox2;
                        case 3:
                            return BrowserVersions.Firefox3;
                        default:
                            return BrowserVersions.Firefox;
                    }
                case "OPERA":
                    switch (majorVersion)
                    {
                        case 9:
                            return BrowserVersions.Opera9;
                        case 10:
                            return BrowserVersions.Opera10;
                        default:
                            return BrowserVersions.Opera;
                    }
                case "APPLEMAC-SAFARI": case "SAFARI":
                    switch (majorVersion)
                    {
                        case 2:
                            return BrowserVersions.Safari2;
                        case 3:
                            return BrowserVersions.Safari3Mac;
                        case 5:
                            return BrowserVersions.Safari3Windows;
                        default:
                            return BrowserVersions.Safari;
                    }
                case "CHROME":
                    return BrowserVersions.Chrome;
                default:
                    return BrowserVersions.Unknown;
            }
        }
    }
}