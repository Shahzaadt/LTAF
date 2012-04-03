using System;
using System.Globalization;
using System.Collections;
using System.Diagnostics;

namespace LTAF.TagParser
{
    internal sealed class Parser
    {
		private bool _hasInvalidMarkup;
		private TagDocument _document;
		private bool _ignoreCase = true;

		public bool IgnoreCase {
			get {
				return _ignoreCase;
			}
			set {
				_ignoreCase = value;
			}
		}

		public bool HasInvalidMarkup {
			get {
				return _hasInvalidMarkup;
			}
		}
		
		public Parser() {
			
		}
		
		public TagDocument Parse(string text, int startIndex) {
			return this.Parse(text, startIndex, false);
		}

		public TagDocument Parse(string text, int startIndex, bool justFirstTag) {
			_document = new TagDocument(text);
			NodeReader reader = new NodeReader(text, startIndex);
			NodeContainer current = _document;
			Node node = reader.ReadNext();
			int nodeCount = 0;
			while(node != null) {
				if (node is ElementCloseTag) {
					if (string.Compare(node.Name, current.Name, _ignoreCase, CultureInfo.InvariantCulture)==0) {
						((Element)current).SetElementPosition(current.StartPosition, current.EndPosition, node.StartPosition, node.EndPosition);
						current = current.Parent;
					}
					else {
						// oops, we should fix the document here
						NodeContainer temp = current.Parent;
						ArrayList list = new ArrayList();
						list.Add(current);
						while(temp != null) {
							if (string.Compare(node.Name, temp.Name, _ignoreCase, CultureInfo.InvariantCulture)==0) {
								((Element)temp).SetElementPosition(temp.StartPosition, temp.EndPosition, node.StartPosition, node.EndPosition);
								current = temp.Parent;
								for (int i=list.Count-1; i>=0; i--) {
									Element tempNode = (Element)list[i];
									int index = temp.Nodes.IndexOf(tempNode)+1;
									tempNode.SetIsInlineClosed(true);
									tempNode.SetIsClosed(false);
									for (int child = tempNode.Nodes.Count-1; child >= 0; child--) {
										Node childNode = tempNode.Nodes[child];
										tempNode.Nodes.Remove(childNode);
										childNode.SetParent(temp);
										temp.Nodes.Insert(index, childNode);
									}
								}
								break;
							}
							list.Add(temp);
							temp = temp.Parent;
						}
						if (temp==null) {
							// oops
							// this seems to be a never-opened tag, 
							//   must likely a wrongly nested like <b><i></b></i>
							//   shall we try doing something???
							nodeCount ++;
							InvalidNode textNode = new InvalidNode();
							textNode.SetPosition(node.StartPosition, node.EndPosition);
							textNode._globalIndex = nodeCount;
							current.Nodes.Add(textNode);
							_hasInvalidMarkup = true;
						}
					}
				}
				else {
					current.Nodes.Add(node);
					nodeCount ++;
					node._globalIndex = nodeCount;
					if (node is Element) {
						Element element = (Element)node;
						if (!element.IsInlineClosed) {
							current = element;
						}
					}
				}
				if (justFirstTag) {
					// For wrongly nested documents I might end with more than one node, so
					//		if the user said only one, then remove all of the rest
					if (_document.Nodes.Count > 1) {
						for (int i=_document.Nodes.Count; i>1; i--) {
							_document.Nodes.RemoveAt(i);
						}
					}
					break;
				}
				node = reader.ReadNext();
			}
			_document._nodeCount = nodeCount;
			return _document;
		}
	}
}
