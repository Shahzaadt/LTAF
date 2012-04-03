using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace LTAF.TagParser
{

    internal class NodeReader
    {
		private string _text;
		private int _length;
		private int _pos;

		internal int Pos { get { return _pos; } }

		private Node _previouslyReadNode;

		public NodeReader(string text, int startPosition) {
			_text = text;
			_length = text.Length;
			_pos = startPosition;
		}

		public Node ReadNext() {
			int startText = -1;
			if (_previouslyReadNode != null) {
				Node node = _previouslyReadNode;
				_previouslyReadNode = null;
				return node;
			}
			for (; _pos < _length - 1 /* we do this -1 since there is no possible node that only has 1 char */ ; _pos++) {
				int backupPos = _pos;
				char c = _text[_pos];
				switch(c) {
					case '<':
						if (_text[_pos+1]=='/') {
							ElementCloseTag closeTag = ParseClosingTag();
							if (closeTag == null) {
								//here we should deal with non-element, only Text?
								if (startText == -1) 
									startText = _pos;
							}
							else {
								if (startText != -1) {
									TextNode textNode = CheckForMissingText(startText, backupPos, closeTag);
									if (textNode != null) return textNode;
								}
								return closeTag;
							}
						}
						else if (_text[_pos+1]=='!') {
							Node result = null; 
							if (_pos+3 <_length && _text[_pos+2]=='-' && _text[_pos+3]=='-') {
								result = ParseComment();
							}
							else {
								result = ParseDocumentType();
							}
							if (result == null) {
								if (startText == -1) {
									startText = _pos;
								}
								break;
							}
							else if (startText != -1) {
								TextNode textNode = CheckForMissingText(startText, backupPos, result); //backupPos +1
								if (textNode != null) return textNode;
							}
							return result;
						}
						else if (_text[_pos+1]=='?') {
							ProcessingInstruction piTag = ParseProcessingInstruction();
							if (piTag == null) {
								//here we should deal with non-element, only Text?
								if (startText == -1) 
									startText = _pos;
							}
							else {
								if (startText != -1) {
									TextNode textNode = CheckForMissingText(startText, backupPos, piTag);
									if (textNode != null) return textNode;
								}
								return piTag;
							}
						}
						else {
							Element element = ParseStartTag();
							if (element != null) {
								if (startText != -1) {
									TextNode textNode = CheckForMissingText(startText, backupPos, element);
									if (textNode != null) return textNode;
								}
								return element;
							}
							else {
								//here we should deal with non-element, only Text?
								if (startText == -1) 
									startText = _pos;
							}
						}
						break;
						//					case '\r': case '\n':
						//						// only if this is first character from text it will be ignored
						//						break;
					default:
						if (startText == -1) 
							startText = _pos;
						break;
				}
			}
			if (startText == -1 && _pos == _length - 1) {
				startText = _pos;
				_pos++;
			}
			if (startText != -1) {
				TextNode textNode = CheckForMissingText(startText, _length, null);
				_pos = _length;
				if (textNode != null) return textNode;
			}

			return null;
		}

		
		private void SkipWhiteSpaces(ref int index) {
			for (;index<_length; index++) {
				if (_text[index]==' ' || _text[index]=='\t'|| _text[index]=='\n'  || _text[index]=='\r' ) {
					continue;
				}
				break;
			}
		}

		private Comment ParseComment() {
			int startPos = _pos;
			int i = _pos + 4;
			#region Parse The Comment
			for (; i<_length-2; i++) {
				char c = _text[i];
				if (c=='-' && 
					_text[i+1]=='-' && 
					_text[i+2]=='>') {
					break;
				}
			}
			#endregion
			int length = i - startPos + 3;
			int max = _length - startPos - 1;
			if (length > max) length = max;
			string text = _text.Substring(startPos, length);
			_pos = i + 3;
			Comment comment = new Comment();
			comment.SetName(null, "#Comment");
			comment.SetPosition(startPos, startPos + length);
			return comment;
		}

		private ElementCloseTag ParseClosingTag() {
			int startPos = _pos;
			string prefix = null, localName = null;
			
			int index = _pos+2; // Skip the </

			if (!ParseName(ref index, out prefix, out localName)) return null;

			SkipWhiteSpaces(ref index);

			if (_length <= index) {
				return null;
			}
			char c = _text[index];
			if (c!='>') return null;

			_pos = index+1;
			ElementCloseTag closeTag = new ElementCloseTag();
			closeTag.SetName(prefix, localName);
			closeTag.SetPosition(startPos, index);
			return closeTag;
		}

		private Element ParseStartTag() {
			int startPos = _pos;
			string prefix = null, localName = null;
			
			int index = _pos+1;

			if (!ParseName(ref index, out prefix, out localName)) return null;;

			Element result = new Element();
			result.SetName(prefix, localName);

			char c = _text[index];
			if (c != '>') {
				if (!ParseAttributes(ref index, result.Attributes)) return null;
				c = _text[index];
				if (c=='/') {
					result.SetIsInlineClosed(true);
					index++;
					if (index >= _length) return null;
					c = _text[index];
				}
			}
		
			if (c!='>') {
				// Everything worked, but this IS not a tag, not closed yet
				return null;
			}
			result.SetElementPosition(startPos, index, index, index);
			_pos = index+1;
			return result;
		}


		private DocumentType ParseDocumentType() {
			int startPos = _pos;
			string prefix = null, localName = null;
			
			int index = _pos+2; // Skip the <!
			// Parse the DOCTYPE
			if (!ParseName(ref index, out prefix, out localName)) return null;
			if (string.Compare(localName,"DOCTYPE", true, CultureInfo.InvariantCulture)!=0) return null;

			if (index == _length) return null;
			
			DocumentType docType = new DocumentType();
			docType.SetName(prefix, localName);

			SkipWhiteSpaces(ref index);
			int start = index;
			if (!ParseName(ref index, out prefix, out localName)) return null;
			Attribute attribute = new Attribute(prefix, localName, null);
			attribute.SetPosition(start, index-1);
			docType.Attributes.Add(attribute);

			SkipWhiteSpaces(ref index);

			start = index;
			if (!ParseName(ref index, out prefix, out localName)) return null;
			attribute = new Attribute(prefix, localName, null);
			attribute.SetPosition(start, index-1);
			docType.Attributes.Add(attribute);

			char c = _text[index];
			for (;index< _length; index++) {
				c = _text[index];
				if (c=='>') break;
				if (c=='"' || c=='\'') {
					start = index; // REVIEW: Shall I use +1 to remove the quote?
					for (index++;index< _length; index++) {
						if (_text[index]==c) {
							attribute = new Attribute(null, _text.Substring(start, index-start+1), null);
							attribute.SetPosition(start, index);
							docType.Attributes.Add(attribute);
							break;
						}
					}
				}
			}

			if (c != '>') {
				return null;
			}
			_pos = index+1;

			docType.SetPosition(startPos, index);
			return docType;
		}


		private ProcessingInstruction ParseProcessingInstruction() { 
			int startPos = _pos;
			string prefix = null, localName = null;
			
			int index = _pos+2; // Skip the <?

			if (!ParseName(ref index, out prefix, out localName)) return null;;

			ProcessingInstruction result = new ProcessingInstruction();
			result.SetName(prefix, localName);

			if (!ParseAttributes(ref index, result.Attributes)) return null;

			char c = _text[index];
			if (c=='?') {
				index++;
				if (index >= _length) return null;
				c = _text[index];
			}
			if (c!='>') {
				// Everything worked, but this IS not a PI tag, not closed yet
				return null;
			}
			_pos = index+1;
			result.SetPosition(startPos, index);
			return result;
		}

		#region Private Parsing
		private bool ParseName(ref int nameEnds, out string prefix, out string localName) {
			localName = null;
			int nameStart = nameEnds;
			prefix = null;
			if (nameEnds >= _length) return false;
			char c = _text[nameEnds];
			switch(c) {
				case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z':
				case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm': case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x': case 'y': case 'z': 
				case '_': case ':': {
					NameContinue:
						nameEnds++;
					if (nameEnds >= _length) return false;
					c = _text[nameEnds];
					switch(c) {
						case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z':
						case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm': case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x': case 'y': case 'z':
						case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9':
						case '.': case '-': case '_':
							goto NameContinue;
						case ':':
							if (prefix==null) {
								prefix = _text.Substring(nameStart, nameEnds - nameStart);
							}
							else {
								return false;
							}
							nameStart = nameEnds+1;
							goto NameContinue;
						case ' ': case '\r': case '\n': case '\t':
						case '/': // This signals the name 
						case '>': // of the tag has ended
							break;
						default:
							return false;
					}
					localName = _text.Substring(nameStart, nameEnds - nameStart);
					break;
				}
				default:
					return false;
			}
			return true;
		}

		private bool ParseAttributes(ref int nameEnds, AttributeCollection attributes) {
			nameEnds--;
			AttributeContinue:
			string prefix = null;
			nameEnds++;
			if (nameEnds >= _length) return false;
			char c = _text[nameEnds];
			switch(c) {
				case ' ': case '\r': case '\n': case '\t':
					// Ignore WhiteSpaces
					goto AttributeContinue;
				case '<':
					return false;
				case '>':
					// we are done parsing the tag
					break;
				case '/':
				case '?': // Added for PI
					// next char should be = >
					break;
				case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z':
				case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm': case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x': case 'y': case 'z': 
				case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9':
				case '_': case ':': {
					// this is where the attribute name starts, isnt it?
					int nameStart = nameEnds;
					int attributeStart = nameStart;
					string attributeName = null;
					AttributeNameContinue:
						nameEnds++;
					if (nameEnds >= _length) return false;
					c = _text[nameEnds];
					switch(c) {
						case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z':
						case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm': case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x': case 'y': case 'z': 
						case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9':
						case '.': case '-': case '_': {
							goto AttributeNameContinue;
						}
						case ':':
							if (prefix==null) {
								prefix = _text.Substring(nameStart, nameEnds - nameStart);
							}
							else {
								return false;
							}
							nameStart = nameEnds+1;
							goto AttributeNameContinue;
						case ' ': case '\r': case '\n': case '\t':
						case '>': case '/': case '=': { // This signals the name of the attribute ends
							attributeName = _text.Substring(nameStart, nameEnds - nameStart);
							string value = null;

							#region ParseValue
							nameEnds--;
							ValueStartContinue:
								nameEnds++;
							if (nameEnds >= _length) return false;
							c = _text[nameEnds];
							switch(c) {
								case ' ': case '\r': case '\n': case '\t':
									goto ValueStartContinue;
								case '<': 
									return false;
								case '=': 
									// this is where the attribute value starts, isnt it?
									nameStart = nameEnds+1;
									bool started = false;
									ValueValueContinue:
										nameEnds++;
									if (nameEnds >= _length) return false;
									c = _text[nameEnds];
									switch(c) {
										case ' ': case '\r': case '\n': case '\t':
										case '/': case '>': 
											if (started && c == '/') { // check for things like type=text/javascript
												if (nameEnds < _length && _text[nameEnds + 1]!= '>' && _text[nameEnds + 1]!=' ') {
													goto ValueValueContinue;
												}
											}
											if (!started) { // Ignore leading spaces
												nameStart = nameEnds+1;
												goto ValueValueContinue;
											}
											value = _text.Substring(nameStart, nameEnds - nameStart);
											break;
										case '\"': case '\'':
											nameStart++;
											while(nameEnds < _length) {
												nameEnds++;
												if (nameEnds >= _length) return false;
												if (_text[nameEnds] == c) {
													break;
												}
											}
											if (nameEnds - nameStart>= 0 ) {
												value = _text.Substring(nameStart, nameEnds - nameStart);
												nameEnds++;
												if (nameEnds >= _length) return false;
												c=_text[nameEnds];
											}
											else {
												Debug.Assert(false,"Something went wrong here");
											}
											break;
										default: {
											started=true;
											goto ValueValueContinue;
										}
									}
									break;
								case '>':
									break;
								default: // This attribute has no value ie: checked
									nameEnds--;
									c = _text[nameEnds];
									break;
							}

							#endregion
							Attribute attribute = new Attribute(prefix, attributeName, value);
							attribute.SetPosition(attributeStart, nameEnds-1);
							attributes.Add(attribute);
							break;
						}
						default:
							return false;
					}
					break;
				}
				default: 
					return false;
			}
			if (c==' ' || c=='\r' || c=='\n' || c=='\t') {
				goto AttributeContinue;
			}

			return true;
		}

		private TextNode CheckForMissingText(int startText, int endText, Node nextNode) {
			for (int textIndex = startText; textIndex < endText; textIndex++) {
				char tempChar = _text[textIndex];
				if (tempChar != '\r' && tempChar != '\n' && tempChar != ' '  && tempChar != '\t') {
					TextNode textNode = new TextNode();
					textNode.SetPosition(startText, endText-1);
					startText = -1;
					_previouslyReadNode = nextNode;
					return textNode;
				}
			}
			return null;
		}
		#endregion

	}
}
