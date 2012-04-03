using System;
using System.Collections.Generic;

namespace LTAF
{
    /// <summary>
    /// Delegate used to create new HtmlElements
    /// </summary>
    /// <param name="tag">HTML tag of the element</param>
    /// <param name="attributes">Attributes defined on the element</param>
    /// <param name="parent">Parent HtmlElement</param>
    /// <param name="page">Page containing the element</param>
    /// <returns>HtmlElement</returns>
    public delegate HtmlElement HtmlElementCreator(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page);

    /// <summary>
    /// Factory used to create new instances of types derived from HtmlElement
    /// that correspond to different HTML tags
    /// </summary>
    public static class HtmlElementFactory
    {
        /// <summary>
        /// Dictionary mapping tag names to HtmlElementCreator delegates used to
        /// construct them
        /// </summary>
        private static Dictionary<string, HtmlElementCreator> _elementFactory;

        /// <summary>
        /// Initialize the factory and register the types defined in
        /// LTAF
        /// </summary>
        static HtmlElementFactory()
        {
            _elementFactory = new Dictionary<string, HtmlElementCreator>(StringComparer.OrdinalIgnoreCase);
            

            // Register all the types defined in LTAF
            Register("a", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlAnchorElement(attributes, parent, page); });
            Register("input", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlInputElement(attributes, parent, page); });
            Register("img", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlImageElement(attributes, parent, page); });
            Register("select", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlSelectElement(attributes, parent, page); });
            Register("option", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
            { return new HtmlOptionElement(attributes, parent, page); });
            Register("table", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlTableElement(attributes, parent, page); });
            Register("tr", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
                { return new HtmlTableRowElement(attributes, parent, page); });
            Register("textarea", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
            { return new HtmlTextAreaElement(attributes, parent, page); });
            Register("form", delegate(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
            { return new HtmlFormElement(attributes, parent, page); });
        }

        /// <summary>
        /// Associate an HTML tag with an HtmlElementCreator delegate used to
        /// construct a corresponding derived HtmlElement instance
        /// </summary>
        /// <param name="tag">HTML tag</param>
        /// <param name="creator">
        /// HtmlElementCreator used to create an HtmlElement for this type of tag
        /// </param>
        public static void Register(string tag, HtmlElementCreator creator)
        {
            // Ensure parameters
            if (tag == null)
                throw new ArgumentNullException("tag", "tag cannot be null!");
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("tag cannot be empty!", "tag");
            if (creator == null)
                throw new ArgumentNullException("creator", "creator cannot be null!");

            // Add the tag to the element factory
            _elementFactory[tag] = creator;
        }

        /// <summary>
        /// Create a new HtmlElement
        /// </summary>
        /// <param name="tag">HTML tag of the element</param>
        /// <param name="attributes">Attributes defined on the element</param>
        /// <param name="parent">Parent HtmlElement</param>
        /// <param name="page">Page containing the element</param>
        /// <returns>HtmlElement</returns>
        internal static HtmlElement Create(string tag, IDictionary<string, string> attributes, HtmlElement parent, HtmlPage page)
        {
            HtmlElement element = null;

            // Try to use an HtmlElementCreator
            HtmlElementCreator creator = null;
            if (_elementFactory.TryGetValue(tag, out creator))
            {
                // Ignore any exceptions raised during creation
                try { element = creator(tag, attributes, parent, page); }
                catch { }
            }

            // If we don't have a creator for the element or it failed to create,
            // create a default HtmlElement
            if (element == null)
                element = new HtmlElement(tag, attributes, parent, page);

            return element;
        }
    }
}