using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace LTAF.Engine
{
    /// <summary>
    /// Class that encapsulates config options passed via query string.
    /// </summary>
    internal class QueryStringParameters
    {
        public bool Run { get; set; }
        public bool Filter { get; set; }
        public bool SkipFail { get; set; }
        public bool WriteLog { get; set; }
        public bool ShowConsole { get; set; }
        public string Tag { get; set; }
        public WebTestLogDetail LogDetail { get; set; }

        public QueryStringParameters()
        {
            LogDetail = WebTestLogDetail.Default;
            ShowConsole = true;
        }

        /// <summary>
        /// Loads settings from query string
        /// </summary>
        public void LoadFromQueryString(NameValueCollection querystring)
        {
            this.Tag = querystring["tag"];
            bool hasTag = !String.IsNullOrEmpty(Tag);
            
            bool run = false;
            bool.TryParse(querystring["run"], out run);
            this.Run = run;

            bool filter = false;
            bool.TryParse(querystring["filter"], out filter);
            this.Filter = filter;

            bool skipfail = false;
            if (bool.TryParse(querystring["skipfail"], out skipfail) && skipfail)
            {
                Tag = hasTag ?
                    string.Format("({0})-Fail", Tag) :
                    "!Fail";
            }
            this.SkipFail = skipfail;

            this.LogDetail = WebTestLogDetail.Default;

            bool concise = false;
            if (bool.TryParse(querystring["concise"], out concise) && concise)
            {
                this.LogDetail = WebTestLogDetail.Concise;
            }

            bool verbose = false;
            if (bool.TryParse(querystring["verbose"], out verbose) && verbose)
            {
                this.LogDetail = WebTestLogDetail.Verbose;
            }

            bool writeLog = false;
            bool.TryParse(querystring["log"], out writeLog);
            this.WriteLog = writeLog;

            bool showConsole = true;
            if (querystring.AllKeys.Contains("console"))
            {
                bool.TryParse(querystring["console"], out showConsole);
            }
            this.ShowConsole = showConsole;
        }
    }
}
