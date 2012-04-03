using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LTAF.Runner
{
    public class RunParameters
    {
        public RunParameters()
        {
            Browser = new IEBrowser();
            ExecutionTimeout = 1800000;//30*60*1000
        }

        public void Parse(string[] parameters)
        {
            foreach (string p in parameters)
            {
                if (p == "/?")
                {
                    this.PrintHelp = true;
                }
				else
				{
					Match m = Regex.Match(p, @"/(?<name>.*?):(?<value>.*?)(\/|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (m.Success)
                    {
                        string paramValue = m.Groups["value"].Value;
                        string paramName = m.Groups["name"].Value;
                        SetProperties(paramName, paramValue);
                    }
                    else
                    {
                        throw new ArgumentException(String.Format("Unknwon parameter sequence '{0}'", p));
                    }
				}
            }
        }

        private void SetProperties(string paramName, string paramValue)
        {
            switch (paramName.ToLowerInvariant())
            {
                case "path":
                    this.WebSitePath = paramValue;
                    break;
                case "browser":
                    this.Browser = GetBrowser(paramValue);
                    break;
                case "tag":
                    this.TagName = paramValue;
                    break;
                case "timeout":
                    int temptimeout;
                    if (int.TryParse(paramValue, out temptimeout))
                    {
                        this.ExecutionTimeout = temptimeout*60000;
                    }
                    else
                    {
                        throw new ArgumentException("Timeout argument must be an integer.");
                    }
                    
                    break;
                default:
                    throw new ArgumentException(String.Format("Unknwon parameter '{0}'", paramName));
            }
        }

        private Browser GetBrowser(string paramValue)
        {
            switch (paramValue.ToLowerInvariant())
            {
                case "ie": case "internetexplorer":
                    return new IEBrowser();
                case "ff": case "firefox":
                    return new FireFoxBrowser();
                default:
                    throw new ArgumentException(String.Format("Uknown browser '{0}'", paramValue));
            }
        }

        public bool PrintHelp
        {
            get;
            set;
        }

		public string WebSitePath
		{
			get;
			set;
		}

        public Browser Browser
        {
            get;
            set;
        }

        public string TagName
        {
            get;
            set;
        }

        public int ExecutionTimeout
        {
            get;
            set;
        }

    }
}