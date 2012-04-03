using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents a row of an html table. The 'tr' html element.
    /// </summary>
    public class HtmlTableRowElement: HtmlElement
    {
        internal HtmlTableRowElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage): base("tr", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Cells
        /// </summary>
        public HtmlElementCollection Cells
        {
            get
            {
                return this.ChildElements;
            }
        }
    }
}