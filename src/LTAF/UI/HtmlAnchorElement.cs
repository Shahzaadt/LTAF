using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'anchor' html element.
    /// </summary>
    public class HtmlAnchorElement: HtmlElement
    {
        internal HtmlAnchorElement(
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage)
            : base("a", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public new HtmlAnchorElementAttributeReader CachedAttributes
        {
            get
            {
                _attributeReader = _attributeReader as HtmlAnchorElementAttributeReader;
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlAnchorElementAttributeReader(this);
                }

                return (HtmlAnchorElementAttributeReader)_attributeReader;
            }
        }

        /// <summary>
        /// Get the attributes for the anchor element
        /// </summary>
        /// <returns>HtmlAnchorElementAttributeReader</returns>
        public new HtmlAnchorElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlAnchorElementAttributeReader(this);
            return (HtmlAnchorElementAttributeReader)_attributeReader;
        }
    }
}
