//------------------------------------------------------------------------------
// <copyright file="TextNode.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    internal class TextNode : Node
    {

        internal TextNode() {
        }

        public override NodeType Type {
            get {
                return NodeType.Text;
            }
        }

    }
}
