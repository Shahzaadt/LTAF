using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace LTAF.Infrastructure
{
    public class WebServer : IDisposable
    {
        public static WebServer Create(WebServerType type)
        {
            return Create(type, new WebServerSettings());
        }

        public static WebServer Create(WebServerType type, WebServerSettings settings) 
        {
            WebServer server = null;
            if (type == WebServerType.IIS)
            {
                server = new WebServerIIS(settings);
            }
            else
            {
                server = new WebServerIISExpress(settings);
            }

            return server;
        }

        protected const string DEFAULT_APPPOOL_NAME = "DefaultAppPool";
        protected const string DEFAULT_WEBSITE_NAME = "Default Web Site";
        protected WebServerType _type = WebServerType.IISExpress;
        protected Version _version = null;
        protected ServerManager _serverManager = null;
        protected string _rootPhysicalPath = "";
        protected string _hostName = "";

        public WebServerType Type
        {
            get
            {
                return this._type;
            }
        }

        public Version Version
        {
            get
            {
                return this._version;
            }
        }

        public string HostName
        {
            get
            {
                return this._hostName;
            }
        }

        public string RootPhysicalPath
        {
            get
            {
                return this._rootPhysicalPath;
            }
        }

        public ServerManager ServerManager
        {
            get
            {
                return this._serverManager;
            }
        }

        public ApplicationPool DefaultAppPool
        {
            get
            {
                return this._serverManager.ApplicationPools[this._serverManager.ApplicationDefaults.ApplicationPoolName];
            }
        }

        public Site DefaultWebSite
        {
            get
            {
                return this._serverManager.Sites[DEFAULT_WEBSITE_NAME];
            }
        }

        protected virtual void DisposeWebServer(bool disposing)
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Reset()
        {
        }

        #region IDisposable

        public void Dispose()
        {
            DisposeWebServer(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        ~WebServer()
        {
            DisposeWebServer(false);
        }
    }
}
