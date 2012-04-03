using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LTAF.UnitTests
{
    public static class Utils
    {
        public static string TrimSpaceBetweenTags(string html)
        {
            html = Regex.Replace(html, @">\s+", ">");
            html = Regex.Replace(html, @"\s+<", "<");
            return html;
        }

    }
}
