using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Collections.ObjectModel;
using LTAF.TagParser;

namespace LTAF
{
    /// <summary>
    /// class that is in charge of building the dom of the client
    /// </summary>
    internal sealed class HtmlElementBuilder
    {
        private HtmlPage _parentPage;
        private string _htmlSource;
        private Stack<HtmlElement> _htmlElementStack;
        private bool _ensureValidMarkup;
        private Node _firstInvalidNode;
        private Dictionary<string, int> _tagIndeces;

        internal HtmlElementBuilder(HtmlPage parentPage)
        {
            _ensureValidMarkup = false;
            _parentPage = parentPage;
            _ensureValidMarkup = false;
            _tagIndeces = new Dictionary<string, int>();
        }

        public bool EnsureValidMarkup
        {
            get { return _ensureValidMarkup; }
            set { _ensureValidMarkup = value; }
        }

        #region CreateElement
        public HtmlElement CreateElement(string html)
        {
            _tagIndeces.Clear();
            _firstInvalidNode = null;
            _htmlSource = html;
            _htmlElementStack = new Stack<HtmlElement>();
            _htmlElementStack.Push(null);

                
            Parser parser = new Parser();
            parser.IgnoreCase = true;
            
            TagDocument tagDocument = parser.Parse(html, 0, false);

            if (tagDocument.Nodes.Count > 1)
            {
                throw new WebTestException("More than one root element is not allowed in the source html when building an HtmlElement.");
            }

            HtmlElement newElement = GenerateHtmlElement((NodeContainer) tagDocument.Nodes[0]);

            if (_ensureValidMarkup && parser.HasInvalidMarkup)
            {
                throw new WebTestException(String.Format("The parsed html source has invalid markup at start position {0}.", _firstInvalidNode.StartPosition));
            }

            return newElement;
        }
        #endregion

        #region GenerateChildElements
        private IList<HtmlElement> GenerateChildElements(NodeContainer nodeContainer)
        {
            List<HtmlElement> elements = new List<HtmlElement>();
            HtmlElement previousElement = null;
            foreach (Node childNode in nodeContainer.Nodes)
            {
                HtmlElement newElement = null;
                if (childNode.Type == NodeType.Element)
                {
                    if (childNode.Name.Equals("script", StringComparison.InvariantCultureIgnoreCase))
                    {
                        newElement = GenerateHtmlScriptElement((NodeContainer)childNode);
                    }
                    else
                    {
                        newElement = GenerateHtmlElement((NodeContainer)childNode);
                    }

                    newElement.PreviousSibling = previousElement;
                    if (previousElement != null)
                    {
                        previousElement.NextSibling = newElement;
                    }

                    elements.Add(newElement);
                    previousElement = newElement;
                }
                else if (childNode.Type == NodeType.Text || childNode.Type == NodeType.Comment)
                {
                    _htmlElementStack.Peek().AppendInnerText(childNode.GetText().Trim());
                }
                else if (childNode.Type == NodeType.Invalid)
                {
                    _firstInvalidNode = childNode;
                }
            }
            return elements;
        }
        #endregion

        #region GenerateHtmlElement
        private HtmlElement GenerateHtmlElement(NodeContainer node)
        {
            string tagName = node.Name.ToLowerInvariant();
            IDictionary<string, string> attributes = GenerateAttributeDictionary(node);
            HtmlElement parentElement = _htmlElementStack.Peek();

            HtmlElement newElement = HtmlElementFactory.Create(tagName, attributes, parentElement, _parentPage);
            newElement.TagNameIndex = GetTagIndex(tagName);
            newElement.StartPosition = node.StartPosition;
            newElement.EndPosition = node.EndPosition;

            _htmlElementStack.Push(newElement);
            newElement.SetChildElements(GenerateChildElements(node));
            _htmlElementStack.Pop();
            
            return newElement;
        }
        #endregion

        #region GetTagIndex
        private int GetTagIndex(string tagName)
        {
            if (_tagIndeces.ContainsKey(tagName))
            {
                return ++_tagIndeces[tagName];
            }
            else
            {
                _tagIndeces.Add(tagName, 0);
                return 0;
            }
        }
        #endregion

        #region GenerateAttributeDictionary
        private IDictionary<string, string> GenerateAttributeDictionary(NodeContainer node)
        {
            Dictionary<string, string> attributesDic = new Dictionary<string, string>();
            foreach (TagParser.Attribute attribute in node.Attributes)
            {
                string key = attribute.Name.ToLowerInvariant();
                if (!attributesDic.ContainsKey(key))
                {
                    attributesDic.Add(key, attribute.Value);
                }
            }
            return attributesDic;
        }
        #endregion

        #region GenerateHtmlScriptElement
        private HtmlElement GenerateHtmlScriptElement(NodeContainer node)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (Node childNode in node.Nodes)
            {
                scriptBuilder.Append(childNode.GetText());
            }

            HtmlElement script = new HtmlElement(
                node.Name,
                GenerateAttributeDictionary(node),
                _htmlElementStack.Peek(), 
                _parentPage);
            
            script.AppendInnerText(scriptBuilder.ToString());
            script.SetChildElements(new List<HtmlElement>());

            return script;
        }
        #endregion
    }    
}
