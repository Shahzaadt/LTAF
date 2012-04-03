using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Class that represents the 'select' html element.
    /// </summary>
    public class HtmlSelectElement: HtmlElement
    {
        internal HtmlSelectElement(
                IDictionary<string, string> attributes,
                HtmlElement parentElement,
                HtmlPage parentPage)
            : base("select", attributes, parentElement, parentPage)
        {
        }

        /// <summary>
        /// CanSelectIndex
        /// </summary>
        protected override bool CanSelectIndex
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public new HtmlSelectElementAttributeReader CachedAttributes
        {
            get
            {
                _attributeReader = _attributeReader as HtmlSelectElementAttributeReader;
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlSelectElementAttributeReader(this);
                }

                return (HtmlSelectElementAttributeReader)_attributeReader;
            }
        }

        /// <summary>
        /// Get the attributes for the select element
        /// </summary>
        /// <returns>HtmlSelectElementAttributeReader</returns>
        public new HtmlSelectElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlSelectElementAttributeReader(this);
            return (HtmlSelectElementAttributeReader)_attributeReader;
        }
    }
}
