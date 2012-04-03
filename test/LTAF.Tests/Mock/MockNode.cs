using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.UnitTests
{
	internal class MockNode: LTAF.TagParser.Node
	{
		public override LTAF.TagParser.NodeType Type
		{
			get { throw new NotImplementedException("Method is not implemented."); }
		}

	}
}
