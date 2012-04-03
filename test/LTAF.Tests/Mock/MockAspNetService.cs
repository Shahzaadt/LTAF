using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;
using System.Web.UI;
using System.Collections.Specialized;

namespace LTAF.UnitTests
{
    public class MockAspNetService: IAspNetPageService
    {
        private bool _isPostBack;
        private string _applicationPath;
        private NameValueCollection _queryString;
        private NameValueCollection _requestForm;
        public Dictionary<string, Control> FindControlResults { get; set; }
        public List<string> ClientScriptBlocks { get; set; }

        public MockAspNetService()
        {
            FindControlResults = new Dictionary<string, Control>();
            FindControlResults.Add("DriverPageScriptManager", new ScriptManager());
            ClientScriptBlocks = new List<string>();
            _queryString = new NameValueCollection();
            _requestForm = new NameValueCollection();
        }

        public bool GetIsPostBack(System.Web.UI.Page p)
        {
            return _isPostBack;
        }

        public void SetIsPostBack(bool isPostback)
        {
            _isPostBack = isPostback;
        }

        public string GetApplicationPath(System.Web.UI.Page p)
        {
            return _applicationPath;
        }

        public void SetApplicationPath(string appPath)
        {
            _applicationPath = appPath;
        }

        public Control FindControl(Page p, string controlId)
        {
            return FindControlResults[controlId];
        }

        public string GetClientScriptWebResourceUrl(Page p, Type type, string resource)
        {
            return "foobar";
        }

        public void RegisterClientScriptBlock(Page p, Type type, string key, string script, bool addScriptTags)
        {
            ClientScriptBlocks.Add(script);
        }

        public NameValueCollection GetQueryString(Page p)
        {
            return _queryString;
        }

        public void SetQueryString(NameValueCollection qs)
        {
            _queryString = qs;
        }

        public NameValueCollection GetRequestForm(Page p)
        {
            return _requestForm;
        }

        public void SetRequestForm(NameValueCollection col)
        {
            _requestForm = col;
        }

        public string GetBrowserName(Page p)
        {
            return "IE";
        }

        public int GetBrowserMajorVersion(Page p)
        {
            return 6;
        }

      
    }
}
