//------------------------------------------------------------------------------
// <copyright file="NodeContainer.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    internal abstract class NodeContainer : Node
    {
        private AttributeCollection _attributes;
        private NodeCollection _nodes;

        internal NodeContainer() {

        }

        public AttributeCollection Attributes {
            get {
                if (_attributes == null) {
                    _attributes = new AttributeCollection(this);
                }
                return _attributes;
            }
        }

        public bool HasAttributes {
            get {
                return (_attributes == null || _attributes.Count == 0) ? false : true;
            }
        }

        public bool HasChildNodes {
            get {
                return (_nodes == null || _nodes.Count == 0) ? false : true;
            }
        }

        public NodeCollection Nodes {
            get {
                if (_nodes == null) {
                    _nodes = new NodeCollection(this);
                }
                return _nodes;
            }
        }

    }

}
