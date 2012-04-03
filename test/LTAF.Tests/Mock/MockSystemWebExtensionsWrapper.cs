using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Engine;

namespace LTAF.UnitTests
{
    internal class MockSystemWebExtensionsWrapper: SystemWebExtensionsWrapper
    {
        public List<string> StartupScriptBlocks { get; set; }

        public MockSystemWebExtensionsWrapper(IAspNetPageService aspNetPage): base(aspNetPage)
        {
            StartupScriptBlocks = new List<string>();
        }

        public override void RegisterStartupScript(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            StartupScriptBlocks.Add(script);
        }
    }
}
