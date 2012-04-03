//------------------------------------------------------------------------------
// <copyright file="ElementCloseTag.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{
    // </Close>
    internal class ElementCloseTag : Node {

        public override NodeType Type {
            get {
                return NodeType.CloseTag;
            }
        }

    }
}
