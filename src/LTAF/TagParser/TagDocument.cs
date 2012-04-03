//------------------------------------------------------------------------------
// <copyright file="TagDocument.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    internal class TagDocument : NodeContainer
    {

        private string _text;
        internal int _nodeCount;

        internal TagDocument(string text) {
            _text = text;
        }

        public override TagDocument Document {
            get {
                return this;
            }
        }

        public int NodeCount {
            get {
                return _nodeCount;
            }
        }

        public string Text {
            get {
                return _text;
            }
        }

        public override NodeType Type {
            get {
                return NodeType.Document;
            }
        }

    }
}
