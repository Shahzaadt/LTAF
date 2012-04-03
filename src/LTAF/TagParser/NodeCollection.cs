//------------------------------------------------------------------------------
// <copyright file="NodeCollection.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    internal class NodeCollection : CollectionBase
    {
        private NodeContainer _parent;

        internal NodeCollection(NodeContainer parent) {
            this._parent = parent;
        }

        public Node this[int index] {
            get {
                return (Node)this.InnerList[index];
            }
        }

        internal void Add(Node node) {
            node.SetParent(_parent);
            this.InnerList.Add(node);
        }

        public int IndexOf(Node node) {
            return this.InnerList.IndexOf(node);
        }

        internal void Insert(int index, Node node) {
            this.InnerList.Insert(index, node);
        }

        internal void Remove(Node node) {
            this.InnerList.Remove(node);
        }

    }

}
