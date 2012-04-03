using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'select' html element.
    /// </summary>
    public class HtmlOptionElement : HtmlElement
    {
        internal HtmlOptionElement(
                IDictionary<string, string> attributes,
                HtmlElement parentElement,
                HtmlPage parentPage)
            : base("option", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public new HtmlOptionElementAttributeReader CachedAttributes
        {
            get
            {
                _attributeReader = _attributeReader as HtmlOptionElementAttributeReader;
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlOptionElementAttributeReader(this);
                }

                return (HtmlOptionElementAttributeReader)_attributeReader;
            }
        }

        /// <summary>
        /// Get the attributes for the select element
        /// </summary>
        /// <returns>HtmlSelectElementAttributeReader</returns>
        public new HtmlOptionElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlOptionElementAttributeReader(this);
            return (HtmlOptionElementAttributeReader)_attributeReader;
        }
    }
}
