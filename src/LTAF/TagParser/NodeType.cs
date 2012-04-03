//------------------------------------------------------------------------------
// <copyright file="NodeType.cs" author="CarlosAg">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.TagParser
{

    internal enum NodeType
    {
        Element = 0,
        Text = 1,
        Attribute = 2,
        CloseTag = 3,
        Comment = 4,
        ProcessingInstruction = 5,
        Document = 6,
        Invalid = 99
    }

}
