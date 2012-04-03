//------------------------------------------------------------------------------
// <copyright file="DocumentType.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{
    // <!DOCTYPE HTML PUBLIC "URL" "URL" >
    internal class DocumentType : NodeContainer
    {

        internal DocumentType() {
        }

        public override NodeType Type {
            get {
                return NodeType.ProcessingInstruction;
            }
        }

    }
}
