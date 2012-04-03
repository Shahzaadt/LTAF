using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Security;

[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("LTAF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Lightweight Test Automation Framework")]
[assembly: AssemblyCopyright("Copyright 2010 Outercurve Foundation")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

[assembly: InternalsVisibleTo("LTAF.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d5f3084974526b169ad6c9a04d9360fe14a72f8851f52cc6caddf242a8f657fcd0e815bbacddcd1705395ff6201397aed66dafffd345bf1674a8253c8fb34c360b6696e0a5fd451889cf3bc3a5484dfe51ac55d848a8cd2d54b1be3fb0bbb29582fec7c94f20c4355b4e92ffcb2c09c7d374960acb212ea37ea2e1220e3fb0c6")]

//scripts
[assembly: WebResource("LTAF.Engine.Resources.TestcaseExecutor.js", "text/javascript", PerformSubstitution = true)]


// images/css
[assembly: WebResource("LTAF.Engine.Resources.driver.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("LTAF.Engine.Resources.spinner.gif", "image/gif")]
[assembly: WebResource("LTAF.Engine.Resources.helpicon.gif", "image/gif")]
[assembly: WebResource("LTAF.Engine.Resources.testPassed.gif", "image/gif")]
[assembly: WebResource("LTAF.Engine.Resources.testFailed.gif", "image/gif")]

// pages
[assembly: WebResource("LTAF.Engine.Resources.StartUpPage.htm", "text/html", PerformSubstitution = true)]
[assembly: WebResource("LTAF.Engine.Resources.LogSuccess.htm", "text/html")]
[assembly: WebResource("LTAF.Engine.Resources.LogErrorFrameSet.htm", "text/html", PerformSubstitution = true)]
[assembly: WebResource("LTAF.Engine.Resources.LogErrorStackTrace.htm", "text/html")]
[assembly: WebResource("LTAF.Engine.Resources.HelpPage.htm", "text/html")]
