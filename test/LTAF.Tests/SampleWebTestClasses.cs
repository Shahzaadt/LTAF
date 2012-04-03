using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.UnitTests
{
    [WebTestClass]
    public class SampleWebTestClass
    {
        [WebTestMethod]
        [WebTestTag("Tag1")]
        public void SampleWebTestMethod1()
        {

        }

        [WebTestMethod]
        [WebTestTag("Tag2")]
        public void SampleWebTestMethod2()
        {

        }
    }
}
