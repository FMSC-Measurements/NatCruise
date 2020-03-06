using FluentAssertions;
using FScruiser.XF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.XF.Pages
{
    public class AskKpiPage_test : TestBase
    {
        public AskKpiPage_test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("0", 0, 0, 0, (string)null)]
        [InlineData("5", 0, 0, 5, (string)null)]

        [InlineData("5", 6, 0, null, "Value Must be Greater or Equal to 6")]
        [InlineData("6", 6, 0, 6, null)]

        [InlineData("5", 0, 5, 5, null)]
        [InlineData("6", 0, 5, null, "Value Must be Less Than or Equal to 5")]

        [InlineData("6", 5, 10, 6, null)]
        [InlineData("9", 5, 10, 9, null)]
        [InlineData("101", 100, 10, 101, null)]//ignore max if less than min
        public void CheckInput_test(string input, int minKPI, int maxKPI, int? expectedKpi, string expectedErrorMessage)
        {
            var result = AskKpiPage.CheckInput(input, minKPI, maxKPI, out var errormessage);
            
            if(expectedErrorMessage != null)
            {
                result.Should().BeNull();
                if (expectedErrorMessage == "something")
                {
                    Output.WriteLine(errormessage);
                }
                else
                {
                    errormessage.Should().Be(expectedErrorMessage);
                }
            }

            if(expectedKpi != null)
            {
                result.KPI.Should().Be(expectedKpi);
            }
        }

        [Fact]
        public void CheckInput_test_stm()
        {
            var result = AskKpiPage.CheckInput("STM", null, null, out var errormessage);
            result.IsSTM.Should().BeTrue();

        }
    }
}
