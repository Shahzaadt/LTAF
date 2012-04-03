using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Collections.Specialized;

namespace LTAF
{
    /// <summary>
    /// Abstraction of the page services.
    /// </summary>
    internal interface IAspNetPageService
    {
        bool GetIsPostBack(Page p);
        string GetApplicationPath(Page p);
        string GetClientScriptWebResourceUrl(Page p, Type type, string resource);
        Control FindControl(Page p, string controlId);
        void RegisterClientScriptBlock(Page p, Type type, string key, string script, bool addScriptTags);
        NameValueCollection GetQueryString(Page p);
        string GetBrowserName(Page p);
        int GetBrowserMajorVersion(Page p);
        NameValueCollection GetRequestForm(Page p);
    }
}
