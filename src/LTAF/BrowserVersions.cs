using System;
using System.Web;
using System.Web.UI;

namespace LTAF
{
    /// <summary>
    /// Browsers support by ASP.NET AJAX
    /// </summary>
    [Flags]
    public enum BrowserVersions
    {
        /// <summary>None</summary>
        None = 0,
        /// <summary>Unknown</summary>
        Unknown = 4096,

        /// <summary>IE6</summary>
        InternetExplorer6 = 1,
        /// <summary>IE7</summary>
        InternetExplorer7 = 2,
        /// <summary>IE8</summary>
        InternetExplorer8 = 1024,
        /// <summary>All IE: InternetExplorer6 | InternetExplorer7 | InternetExplorer8</summary>
        InternetExplorer = InternetExplorer6 | InternetExplorer7 | InternetExplorer8,

        /// <summary>FF1</summary>
        Firefox1 = 4,
        /// <summary>FF2</summary>
        Firefox2 = 8,
        /// <summary>FF3</summary>
        Firefox3 = 16,
        /// <summary>AllFireFox: Firefox1 | Firefox2 | Firefox3</summary>
        Firefox = Firefox1 | Firefox2 | Firefox3,

        /// <summary>Opera9</summary>
        Opera9 = 32,
        /// <summary>Opera10</summary>
        Opera10 = 64,
        /// <summary>All Opera: Opera9 | Opera10</summary>
        Opera = Opera9 | Opera10,

        /// <summary>Safari2</summary>
        Safari2 = 128,
        /// <summary>Safari3Mac</summary>
        Safari3Mac = 256,
        /// <summary>Safari3Windows</summary>
        Safari3Windows = 512,
        /// <summary>Safari3Mac</summary>
        Safari3 = Safari3Mac | Safari3Windows,
        /// <summary>All Safari: Safari2 | Safari3</summary>
        Safari = Safari2 | Safari3,

        /// <summary>Chrome</summary>
        Chrome = 2048,

        /// <summary>All browsres: InternetExplorer | Firefox | Opera | Safari | Chrome</summary>
        All = InternetExplorer | Firefox | Opera | Safari | Chrome,
    }
}