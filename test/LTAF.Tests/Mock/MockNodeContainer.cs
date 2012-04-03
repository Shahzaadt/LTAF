using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.TagParser;

namespace LTAF.UnitTests
{
	internal class MockNodeContainer : NodeContainer
	{
		public override NodeType Type
		{
			get { throw new NotImplementedException(); }
		}
	}
}
