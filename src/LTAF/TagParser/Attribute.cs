//------------------------------------------------------------------------------
// <copyright file="Attribute.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser {

	internal class Attribute : Node {
		private string _value;

		internal Attribute(string prefix, string localName, string value) {
			this.SetName(prefix, localName);
			this._value = value;
		}

		public override NodeType Type {
			get {
				return NodeType.Attribute;
			}
		}

		public string Value {
			get {
				return _value;
			}
		}

	}

}
