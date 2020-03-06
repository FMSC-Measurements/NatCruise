using FScruiser.XF.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.Pages
{
    public class LimitingDistancePage_Test : TestBase
    {
        public LimitingDistancePage_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Ctor_test()
        {
            try
            {
                var instance = new LimitingDistancePage();
            }
            catch(Exception e)
            {
                Debug.Write(e);
            }


        }
    }
}
