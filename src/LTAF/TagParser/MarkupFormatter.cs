using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.TagParser
{
    /// <summary>
    /// Class that can format any tag markup
    /// </summary>
    /// <change date="02/27/2008">Created</change>
    public class MarkupFormatter
    {
        private StringBuilder _builder;
        private string _text;
        private int _indentation;
        private const int MAX_TAG = 62;
        private TagDocument _document;

        /// <summary>
        /// Format the given text
        /// </summary>
        /// <param name="text">The tag markup text to format</param>
        /// <returns>A string with the markup with indentation formatting</returns>
        public string Format(string text)
        {
            // parse the text into a tag document
            Parser parser = new Parser();
            parser.IgnoreCase = true;
            _document = parser.Parse(text, 0, false);

            _builder = new StringBuilder();
            _text = text;
            _indentation = 0;
            DumpNodes(_document.Nodes);
            return _builder.ToString();            
        }

        private void DumpNodes(NodeCollection nodes)
        {
            foreach (Node node in nodes)
            {
                DumpNode(node);
            }
        }

        private string GetIndentationString()
        {
            return new string('\t', _indentation);
        }

        private void DumpNode(Node node)
        {
            StringBuilder wholeTag = new StringBuilder(node.GetText());
            if (!(node is TextNode) && !(node is Comment))
            {
                _builder.Append(GetIndentationString());
               
                if (node is NodeContainer)
                {
                    NodeContainer container = (NodeContainer)node;
                    if (container is Element && container.Nodes.Count == 1 &&
                        container.Nodes[0] is TextNode)
                    {
                        // the container is an element and it only has text inside
                        Element element = (Element)container;
                        StringBuilder innerText = new StringBuilder(element.Nodes[0].GetText());
                        if (innerText.Length < MAX_TAG / 2)
                        {
                            _builder.Append(IndentOpeningTag(element, element.GetOpenTag()));
                            _builder.Append(IndentText(innerText, false));
                            _builder.Append(element.GetCloseTag());
                            _builder.Append('\n');
                            return;
                        }
                    }
                    if (!container.HasChildNodes)
                    {
                        // the element does not have have any children, its only the element tag
                        wholeTag.Replace("\r", "");
                        wholeTag.Replace("\n", "");
                        _builder.Append(IndentOpeningTag(container, wholeTag.ToString()));
                        _builder.Append('\n');
                        return;
                    }
                    if (container is Element)
                    {
                        // a normal element with any number of children.
                        Element element = (Element)container;
                        _builder.Append(IndentOpeningTag(element, element.GetOpenTag()));
                        _builder.Append('\n');

                        _indentation++;
                        DumpNodes(element.Nodes);
                        _indentation--;

                        _builder.Append(GetIndentationString());
                        _builder.Append(element.GetCloseTag());
                        _builder.Append('\n');
                    }
                    else
                    {
                        // it's not an element and it does not have children. Just write it.
                        _builder.Append(wholeTag);
                        _builder.Append('\n');
                    }
                }
                else
                {
                    // it is not a container object, just write it.
                    _builder.Append(wholeTag);
                    _builder.Append('\n');
                    return;
                }
            }
            else
            {
                // it is a textnode or a comment, write it with format.
                _builder.Append(IndentText(wholeTag, true));
                _builder.Append("\r\n");
            }
        }

        private string IndentText(StringBuilder text, bool indentFirstLine)
        {
            StringBuilder r = new StringBuilder();
            bool inNewLine = true;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\r':
                        inNewLine = true;
                        if (i < text.Length && text[i + 1] == '\n')
                        {
                            i++;
                        }
                        if (i == 0)
                        {
                            // Eat the first enter in case it is
                            break;
                        }
                        r.Append('\n');
                        break;
                    case '\n':
                        inNewLine = true;
                        if (i == 0)
                        {
                            // Eat the first enter in case it is
                            break;
                        }
                        r.Append('\n');
                        break;
                    case '\t':
                        if (inNewLine) continue; // Eat Tabs at the beginning of the line
                        r.Append(c);
                        break;
                    default:
                        inNewLine = false;
                        r.Append(c);
                        break;
                }
            }
            bool removing = true;
            bool shouldRemove = false;
            int index = r.Length - 1;
            while (removing)
            {
                char c = r[index];
                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        index--;
                        shouldRemove = true;
                        break;
                    default:
                        removing = false;
                        break;
                }
            }
            if (shouldRemove)
            {
                r.Remove(index + 1, r.Length - index - 1);
            }

            string indent = GetIndentationString();
            if (indentFirstLine)
            {
                return indent + r.Replace("\n", "\n" + indent).ToString();
            }
            return r.Replace("\n", "\n" + indent).ToString();
        }

        private string IndentOpeningTag(NodeContainer container, string text)
        {
            if (!container.HasAttributes)
            {
                // if it doesn't have attributes, just return the open tag
                return text;
            }
            int pos = text.IndexOf(container.Attributes[0].GetText());
            StringBuilder wholeTag = new StringBuilder();
            wholeTag.Append(text.Substring(0, pos));
            
            //clean up the element string
            for (int i = wholeTag.Length - 1; i >= 0; i--)
            {
                if (wholeTag[i] != ' ' && wholeTag[i] != '\t' && wholeTag[i] != '\n' && wholeTag[i] != '\r')
                {
                    if (i == wholeTag.Length - 1) break;
                    wholeTag.Remove(i + 1, wholeTag.Length - i - 1);
                    break;
                }
            }
            string newIndent = new string('\t', _indentation + 1);
            StringBuilder currentRow = new StringBuilder(MAX_TAG);
            bool isFirstRow = true;
            foreach (Attribute a in container.Attributes)
            {
                if (currentRow.Length >= MAX_TAG)
                {
                    if (!isFirstRow)
                    {
                        wholeTag.Append('\n');
                        wholeTag.Append(newIndent);
                    }
                    isFirstRow = false;
                    wholeTag.Append(currentRow);
                    currentRow = new StringBuilder(MAX_TAG);
                }
                currentRow.Append(' ');
                currentRow.Append(a.GetText());
            }

            //add the left over attributes
            if (currentRow.Length > 0)
            {
                if (!isFirstRow && currentRow.Length > 15)
                {
                    wholeTag.Append('\n');
                    wholeTag.Append(newIndent);
                }
                wholeTag.Append(currentRow);
            }

            //add the closing element tag
            string lastAttributeText = container.Attributes[container.Attributes.Count - 1].GetText();
            pos = text.LastIndexOf(lastAttributeText);
            string endTag = text.Substring(pos + lastAttributeText.Length).Replace(" ", "").Replace("\t", "");
            if (endTag == "/>")
            {
                endTag = " " + endTag;
            }
            wholeTag.Append(endTag);
            return wholeTag.ToString();
        }
    }
}
