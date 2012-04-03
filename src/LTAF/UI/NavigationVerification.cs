using System;

namespace LTAF
{
    /// <summary>
    /// How we determine when navigation to a new page has completed
    /// </summary>
    public enum NavigationVerification
    {
        /// <summary>
        /// Use readystate to determine when the page has loaded
        /// </summary>
        Default = 0,

        /// <summary>
        /// Wait for the page and the ASP.NET AJAX libraries to have loaded
        /// </summary>
        AspNetAjax = 1
    }
}