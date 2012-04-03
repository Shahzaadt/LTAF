//------------------------------------------------------------------------------
// <copyright file="Node.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Globalization;

namespace LTAF.TagParser
{

    internal abstract class Node
    {
		private object _tag;
		private NodeContainer _parent;
		private int _startPosition, _endPosition;
		internal int _globalIndex;
		private string _localName;
		private string _prefix;

		public virtual TagDocument Document {
			get {
				if (Parent == null) {
					return null;
				}
				return Parent.Document;
			}
		}

		public int EndPosition {
			get {
				return _endPosition;
			}
		}

		public int GlobalIndex {
			get {
				return _globalIndex;
			}
		}

		public string LocalName {
			get {
				return _localName;
			}
		}

		public string Name {
			get {
				return _prefix==null ? _localName : _prefix + ":" + _localName;
			}
		}

		public NodeContainer Parent {
			get {
				return _parent;
			}
		}

		public string Prefix {
			get {
				return _prefix;
			}
		}

		public int StartPosition {
			get {
				return _startPosition;
			}
		}

		public object Tag {
			get {
				return _tag;
			}
			set {
				_tag = value;
			}
		}

		public abstract NodeType Type {get;}

		public string GetText() {
			if (this._endPosition > 0 && this._startPosition <= this._endPosition) {
				TagDocument doc = this.Document;
				if (doc == null) {
					return string.Empty;
				}
				int length = doc.Text.Length;
				if (length == 0) {
					return "";
				}
				int nodeLength = this._endPosition - this._startPosition + 1;
				if (_startPosition + nodeLength > length) {
					nodeLength = length - _startPosition;
				}
				if (nodeLength <=0) {
					return "";
				}
				return doc.Text.Substring(this._startPosition, nodeLength);
			}
			return string.Empty;
		}

		public string GetText(string text) {
			if (this.EndPosition > 0 && this.StartPosition < this.EndPosition) {
				return text.Substring(this.StartPosition, this.EndPosition - this.StartPosition + 1);
			}
			return string.Empty;
		}

		internal void SetName(string prefix, string localName) {
			this._prefix = prefix;
			this._localName = localName;
		}

		internal void SetParent(NodeContainer parent) {
			this._parent = parent;
		}

		internal void SetPosition(int startPosition, int endPosition) {
			this._startPosition = startPosition;
			this._endPosition = endPosition;
		}
	}
}