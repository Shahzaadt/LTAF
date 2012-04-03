//------------------------------------------------------------------------------
// <copyright file="AttributeCollection.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LTAF.TagParser
{
    internal class AttributeCollection : CollectionBase
    {
        private NodeContainer _parent;

        public AttributeCollection(NodeContainer parent) {
            this._parent = parent;
        }

        public Attribute this[int index] {
            get {
                return (Attribute)this.InnerList[index];
            }
        }

        public NodeContainer Parent {
            get {
                return _parent;
            }
        }

        internal int Add(Attribute attribute) {
            attribute.SetParent(_parent);
            return this.InnerList.Add(attribute);
        }

        public bool Contains(string name, bool ignoreCase) {
            return this.IndexOf(name, ignoreCase) != -1;
        }

        public Attribute GetAttribute(string attributeName) {
            foreach (Attribute attribute in this) {
                if (String.Compare(attribute.Name, attributeName, true, CultureInfo.InvariantCulture) == 0) {
                    return attribute;
                }
            }
            return null;
        }

        public int IndexOf(string name, bool ignoreCase) {
            for (int i = 0; i < this.Count; i++) {
                if (string.Compare(this[i].Name, name, ignoreCase, CultureInfo.InvariantCulture) == 0) {
                    return i;
                }
            }
            return -1;
        }

    }
}
