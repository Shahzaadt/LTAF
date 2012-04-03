using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'textarea' html element.
    /// </summary>
    public class HtmlTextAreaElement : HtmlElement
    {
        internal HtmlTextAreaElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage)
            : base("textarea", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// CanSetText
        /// </summary>
        protected override bool CanSetText
        {
            get
            {
                return true;
            }
        }
    }
}
