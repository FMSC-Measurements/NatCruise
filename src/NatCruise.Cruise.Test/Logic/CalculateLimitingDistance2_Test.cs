using FluentAssertions;
using NatCruise.Cruise.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Logic
{
    public class CalculateLimitingDistance2_Test
    {
        ILimitingDistanceCalculator LimitingDistanceCalculator { get; }

        public CalculateLimitingDistance2_Test()
        {
            LimitingDistanceCalculator = new CalculateLimitingDistance();
        }

        [Theory]
        [InlineData(20.0, 12.0, 0, false, 23.33)]
        [InlineData(20.0, 12.0, 0, true, 23.82)]
        public void TestCalculateVariableRadious(decimal baf, decimal dbh, int slopePCT, bool isFace, decimal expected)
        {
            int sigDec = 3;

            var ld = LimitingDistanceCalculator.Calculate(baf, dbh, slopePCT, true, isFace);
            ld = Math.Round(ld, sigDec);
            expected = Math.Round(expected, 3);
            ld.Should().Be(expected);
        }


        [InlineData(20.0, 1.944)]
        [Theory]
        public void TestCalculatePlotRadiusFactor(decimal baf, decimal expected)
        {
            var prf = CalculateLimitingDistance2.CalculatePlotRadiusFactor(baf);

            prf.Should().Be(expected);
        }
    }
}
