using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LTAF
{
    /// <summary>
    /// Class to encapsulate find params to use in Find methods of HtmlElementCollection.
    /// </summary>
    public class HtmlElementFindParams
    {
        private const string ID_ATTRIBUTE_NAME = "id";
        private int _index;
        private int _indexInternal;
        private string _innerText;
        private string _tagName;
        private HtmlAttributeFindParams _idAttributeFindParams;
        private HtmlAttributeFindParamsCollection _attributes;

        /// <summary>
        /// Public basic constructor.
        /// </summary>
        public HtmlElementFindParams()
        {
            _idAttributeFindParams = new HtmlAttributeFindParams(ID_ATTRIBUTE_NAME, null, MatchMethod.EndsWith);
            _attributes = new HtmlAttributeFindParamsCollection();
        }

        /// <summary>
        /// Constructor to find an HtmlElement by its partial id or name
        /// </summary>
        /// <param name="idEndsWith">The string to match if it appears at the end of the HtmlElement's id or name (case insensitive).</param>
        public HtmlElementFindParams(string idEndsWith)
            : this()
        {
            _idAttributeFindParams.Value = idEndsWith;
        }

        /// <summary>
        /// Constructor to find an HtmlElement by passing an anonymous object whose properties represent the attributes/values you are searching on.
        ///     To change the match method for the attribute use [attributeName]MatchMethod.
        /// </summary>
        /// <param name="FindParams">anonymous object whose properties represent the attributes/values you are searching on</param>
        /// <example>new HtmlElementFindParams(new { id="myId", idMatchMethod=MatchMethod.Literal, @class="blue" })</example>
        public HtmlElementFindParams(object FindParams)
            : this()
        {
            //Convert the object to a dictionary and seperate it to values and match types
            Dictionary<string, object> allValues = ConvertObjectToDictionary(FindParams);
            Dictionary<string, object> attributeMatchMethods =
                allValues.Where(v => v.Key.EndsWith("MatchMethod", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(d => d.Key, d => d.Value, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, object> attributes =
                allValues.Where(v => !attributeMatchMethods.ContainsKey(v.Key))
                .ToDictionary(d => d.Key, d => d.Value, StringComparer.OrdinalIgnoreCase);

            if (attributes.ContainsKey("Id"))
            {
                _idAttributeFindParams.Value = attributes["Id"].ToString();
                if (attributeMatchMethods.ContainsKey("IdMatchMethod"))
                {
                    _idAttributeFindParams.ValueMatchMethod = (MatchMethod)attributeMatchMethods["IdMatchMethod"];
                    attributeMatchMethods.Remove("IdMatchMethod");
                }
                attributes.Remove("Id");
            }

            if (attributes.ContainsKey("tagName"))
            {
                _tagName = attributes["tagName"].ToString();
                _index = 0;
                attributes.Remove("tagName");
            }

            if (attributes.ContainsKey("index"))
            {
                _index = Convert.ToInt32(attributes["index"]);
                attributes.Remove("index");
            }

            if (attributes.ContainsKey("innerText"))
            {
                _innerText = attributes["innerText"].ToString();
                attributes.Remove("innerText");
            }

            foreach (string key in attributes.Keys)
            {

                if (attributeMatchMethods.ContainsKey(key + "MatchMethod"))
                {
                    _attributes.Add(key, attributes[key].ToString(), (MatchMethod)attributeMatchMethods[key + "MatchMethod"]);
                }
                else
                {
                    _attributes.Add(key, attributes[key].ToString());
                }
            }

        }

        /// <summary>
        /// Constructor to find an HtmlElement by its id or name (case insensitive)
        /// </summary>
        /// <param name="id">The id of the element to find</param>
        /// <param name="idMatchMethod">How to treat the string when matching it with the HtmlElement's id or name.</param>
        public HtmlElementFindParams(string id, MatchMethod idMatchMethod)
            : this()
        {
            _idAttributeFindParams.Value = id;
            _idAttributeFindParams.ValueMatchMethod = idMatchMethod;
        }

        /// <summary>
        /// Constructor to find an HtmlElement by its tagname and index within the dom
        /// </summary>
        /// <param name="tagName">The tag name of the element to find (case insensitive).</param>
        /// <param name="index">The index of the element with given tag name from the start of the Dom.</param>
        public HtmlElementFindParams(string tagName, int index)
            : this(tagName, null, index)
        {
        }

        /// <summary>
        /// Constructor to find an HtmlElement by its tagname, its innertext and its index within the dom
        /// </summary>
        /// <param name="tagName">The tag name of the element to find (case insensitive).</param>
        /// <param name="innerText">The inner text of the element to find (case insensitive).</param>
        /// <param name="index">The index of the element with given tag name and inner text from the start of the Dom.</param>
        public HtmlElementFindParams(string tagName, string innerText, int index)
            : this()
        {
            _tagName = tagName;
            _index = index;
            _innerText = innerText;
        }

        /// <summary>
        /// Method for taking an object and converting its properties/values to a dictionary.
        /// </summary>
        /// <param name="values">Object whose properties/values will be converted.</param>
        /// <returns>The dictionary that represents the properties/values.</returns>
        internal static Dictionary<string,object> ConvertObjectToDictionary(object values)
        {
            Dictionary<string, object> dictionary = null;
            if (values != null)
            {
                dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor prop in props)
                {
                    object val = prop.GetValue(values);
                    dictionary.Add(prop.Name, val);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// When multiple elements exists that match, this property indicates the zero-based index of the element to locate.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// Property that we can modify to use when searching. 
        /// </summary>
        internal int IndexInternal
        {
            get { return _indexInternal; }
            set { _indexInternal = value; }
        }

        /// <summary>
        /// The tan name of the element to find.
        /// </summary>
        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        /// <summary>
        /// The inner text of the element to find.
        /// </summary>
        public string InnerText
        {
            get { return _innerText; }
            set { _innerText = value; }
        }

        /// <summary>
        /// Information about how to match the id attribute of the element to find
        /// </summary>
        public HtmlAttributeFindParams IdAttributeFindParams
        {
            get { return _idAttributeFindParams; }
            set { _idAttributeFindParams = value; }
        }

        /// <summary>
        /// A dictionary of attribute/value pair to use when searching for the element.
        /// </summary>
        public HtmlAttributeFindParamsCollection Attributes
        {
            get
            {
                return _attributes;
            }
        }

        /// <summary>
        /// Determines if a given element matches all the parameters contained in this object
        /// </summary>
        internal bool DoesElementMatch(HtmlElement element)
        {
            return MatchId(element)
                && MatchTagName(element)
                && MatchAttributes(element)
                && MatchInnerText(element);
        }

        /// <summary>
        /// Determines if a given element matches the tag name contained in this object
        /// </summary>
        private bool MatchTagName(HtmlElement element)
        {
            if (String.IsNullOrEmpty(this.TagName))
            {
                return true;
            }
            else
            {
                return String.Equals(element.TagName, this.TagName, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Determines if a given element matches the id contained in this object
        /// </summary>
        private bool MatchId(HtmlElement element)
        {
            if (String.IsNullOrEmpty(this.IdAttributeFindParams.Value))
            {
                return true;
            }

            return (MatchSingleAttribute(element, this.IdAttributeFindParams.Name, this.IdAttributeFindParams.Value, this.IdAttributeFindParams.ValueMatchMethod)
                        || MatchSingleAttribute(element, "name", this.IdAttributeFindParams.Value, this.IdAttributeFindParams.ValueMatchMethod));
        }


        /// <summary>
        /// Determines if a given element matches the attributes contained in this object
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool MatchAttributes(HtmlElement element)
        {
            foreach (HtmlAttributeFindParams attributeMatch in this._attributes)
            {
                if (!MatchSingleAttribute(element, attributeMatch.Name, attributeMatch.Value, attributeMatch.ValueMatchMethod))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a given element matches a specific attribute
        /// </summary>
        private bool MatchSingleAttribute(HtmlElement element, string attributeName, string attributeValue, MatchMethod matchMethod)
        {
            bool isAttributeMatched = false;
            if (element.CachedAttributes.Dictionary.ContainsKey(attributeName))
            {
                string realAttributeValue = element.CachedAttributes.Dictionary[attributeName];

                switch (matchMethod)
                {
                    case MatchMethod.Literal:
                        isAttributeMatched = String.Equals(realAttributeValue, attributeValue, StringComparison.OrdinalIgnoreCase);
                        break;
                    case MatchMethod.EndsWith:
                        isAttributeMatched = realAttributeValue.EndsWith(attributeValue, StringComparison.OrdinalIgnoreCase);
                        break;
                    case MatchMethod.Contains:
                        isAttributeMatched = realAttributeValue.Contains(attributeValue);
                        break;
                    case MatchMethod.Regex:
                        isAttributeMatched = Regex.IsMatch(realAttributeValue, attributeValue, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
                        break;
                }
            }

            return isAttributeMatched;
        }


        /// <summary>
        /// Determines if a given element matches the inner text contained in this object
        /// </summary>
        private bool MatchInnerText(HtmlElement element)
        {
            if (String.IsNullOrEmpty(this.InnerText))
            {
                return true;
            }
            else
            {
                return String.Equals(element.CachedInnerText.Trim(), this.InnerText, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
