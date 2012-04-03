//------------------------------------------------------------------------------
// <copyright file="ProcessingInstruction.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    // <?xml version="1.0" ?>
    internal class ProcessingInstruction : NodeContainer
    {

        internal ProcessingInstruction() {
        }

        public override NodeType Type {
            get {
                return NodeType.ProcessingInstruction;
            }
        }

    }
}
