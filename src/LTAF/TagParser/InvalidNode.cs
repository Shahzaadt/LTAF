//------------------------------------------------------------------------------
// <copyright file="InvalidNode.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{
    internal class InvalidNode : Node
    {

        internal InvalidNode() {
        }

        public override NodeType Type {
            get {
                return NodeType.Invalid;
            }
        }

    }
}
