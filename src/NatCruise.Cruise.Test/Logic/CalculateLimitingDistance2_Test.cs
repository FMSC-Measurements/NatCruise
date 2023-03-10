using FluentAssertions;
using NatCruise.Cruise.Logic;
using NatCruise.Test;
using NatCruise.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Logic
{
    public class CalculateLimitingDistance2_Test : TestBase
    {
        ILimitingDistanceCalculator LimitingDistanceCalculator { get; }

        public CalculateLimitingDistance2_Test(ITestOutputHelper output) : base(output)
        {
            LimitingDistanceCalculator = new CalculateLimitingDistance2();
        }

        [Theory]
        [InlineData(20.0, 12.0, 0, false, 23.33)]
        [InlineData(20.0, 12.0, 0, true, 23.82)] // fails, discrepancy due to small difference in to-face correction
        public void TestCalculateVariableRadious(decimal baf, decimal dbh, int slopePCT, bool isFace, decimal expected)
        {
            int sigDec = 3;

            var ld = LimitingDistanceCalculator.Calculate(baf, 0.0m, dbh, slopePCT, true, isFace);
            ld = Math.Round(ld, sigDec);
            expected = Math.Round(expected, 3);
            ld.Should().Be(expected);
        }

        // values from https://www.fs.usda.gov/Internet/FSE_DOCUMENTS/stelprdb5413736.pdf
        [InlineData(10.0, 2.750)]
        [InlineData(20.0, 1.944)]
        [InlineData(30.0, 1.588)]
        [InlineData(40.0, 1.375)]
        [InlineData(60.0, 1.123)]
        [InlineData(75.0, 1.004)]
        [InlineData(80.0, 0.972)]
        [Theory]
        public void TestCalculatePlotRadiusFactor(decimal baf, decimal expected)
        {
            var prf = CalculateLimitingDistance2.CalculatePlotRadiusFactor(baf);

            prf.Should().Be(expected);
        }

        [InlineData(0,9, "1.00")]
        [InlineData(10, 10, "1.01")] // 10 is somewhat an exceptional case
        [InlineData(11, 17, "1.01")]
        [InlineData(18, 22, "1.02")]
        [InlineData(23, 26, "1.03")]
        [InlineData(27, 30, "1.04")]
        [InlineData(31, 33, "1.05")]
        [InlineData(34, 36, "1.06")]
        [InlineData(37, 39, "1.07")]
        [InlineData(40, 42, "1.08")]
        [InlineData(43, 44, "1.09")]
        [InlineData(45, 47, "1.10")]
        [InlineData(48, 49, "1.11")]
        [InlineData(50, 51, "1.12")]
        [InlineData(52, 53, "1.13")]
        [InlineData(54, 55, "1.14")]
        [InlineData(56, 57, "1.15")]
        [InlineData(58, 59, "1.16")]
        [InlineData(60, 61, "1.17")]
        [InlineData(62, 63, "1.18")]
        [InlineData(64, 65, "1.19")]
        [InlineData(66, 67, "1.20")]
        [InlineData(68, 69, "1.21")]
        [InlineData(70, 70, "1.22")]
        [InlineData(71, 72, "1.23")]
        [InlineData(73, 74, "1.24")]
        [InlineData(75, 75, "1.25")]
        [InlineData(76, 77, "1.26")]
        [InlineData(78, 79, "1.27")]
        [InlineData(80, 80, "1.28")]
        [InlineData(81, 82, "1.29")]
        [InlineData(83, 83, "1.30")]
        [InlineData(84, 85, "1.31")]
        [InlineData(86, 86, "1.32")]
        [InlineData(87, 88, "1.33")]
        [InlineData(89, 89, "1.34")]
        [InlineData(90, 91, "1.35")]
        [InlineData(92, 92, "1.36")]
        [InlineData(93, 94, "1.37")]
        [InlineData(95, 95, "1.38")]
        [InlineData(96, 97, "1.39")]
        [InlineData(98, 98, "1.40")]
        [InlineData(99, 99, "1.41")]
        [InlineData(101, 101, "1.42")]
        [Theory]
        public void CalculateSlopeCorrectionFactor_TestRange(int slopePct1, int slopePct2, string expectedValue)
        {
            var dExpectedValue = decimal.Parse(expectedValue);

            var scf1 = CalculateLimitingDistance2.CalculateToSlopeCorrectionFactor(slopePct1);
            scf1.Should().Be(dExpectedValue);

            var scf2 = CalculateLimitingDistance2.CalculateToSlopeCorrectionFactor(slopePct2);
            scf1.Should().Be(dExpectedValue);
        }

        [Fact]
        // scan through slope values between 0 and 200 to see if any SCF calculated values
        // are borderline when it comes to rounding. This is to help identify values we should
        // check to see if they are consistent with the handbook look up tables
        public void CalculateSlopeCorrectionFactor_checkForBorderlineRounding()
        {
            foreach(var i in Enumerable.Range(0, 200))
            {
                decimal slope = i / 100.0m; 
                var slopeSqr = slope * slope;
                var scf = DecimalMath.Sqrt(1.0m + slopeSqr);

                var scfRound = Decimal.Round(scf, 2, MidpointRounding.AwayFromZero);
                var scfRound3 = Decimal.Round(scf, 3, MidpointRounding.AwayFromZero);
                var scfDoubleRound = Decimal.Round(scfRound3, 2, MidpointRounding.AwayFromZero);

                if(scfRound != scfDoubleRound)
                {
                    Output.WriteLine($"{i} r:{scfRound} dr:{scfDoubleRound}");
                }
            }
        }
    }
}
