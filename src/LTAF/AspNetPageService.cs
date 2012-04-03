using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace LTAF
{
    /// <summary>
    /// Wrapper around System.Web.UI.Page for unit test
    /// </summary>
    internal class AspNetPageService : IAspNetPageService
    {
        public bool GetIsPostBack(Page p)
        {
            return p.IsPostBack;
        }

        public string GetApplicationPath(Page p)
        {
            return p.Request.ApplicationPath;
        }

        public Control FindControl(Page p, string controlId)
        {
            return p.FindControl(controlId);
        }

        public string GetClientScriptWebResourceUrl(Page p, Type type, string resource)
        {
            return p.ClientScript.GetWebResourceUrl(type, resource);
        }

        public void RegisterClientScriptBlock(Page p, Type type, string key, string script, bool addScriptTags)
        {
            p.ClientScript.RegisterClientScriptBlock(type, key, script, addScriptTags);
        }

        public System.Collections.Specialized.NameValueCollection GetQueryString(Page p)
        {
            return p.Request.QueryString;
        }

        public string GetBrowserName(Page p)
        {
            return p.Request.Browser.Browser;
        }

        public int GetBrowserMajorVersion(Page p)
        {
            return p.Request.Browser.MajorVersion;
        }

        public System.Collections.Specialized.NameValueCollection GetRequestForm(Page p)
        {
            return p.Request.Form;
        }
    }
}
