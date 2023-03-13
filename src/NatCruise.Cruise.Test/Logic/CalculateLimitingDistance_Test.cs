using FluentAssertions;
using NatCruise.Cruise.Logic;
using NatCruise.Test;
using System;
using Xunit;
using Xunit.Abstractions;

namespace NatCruise.Cruise.Test.Logic
{
    public class CalculateLimitingDistance_Test : TestBase
    {
        ILimitingDistanceCalculator LimitingDistanceCalculator { get; }

        public CalculateLimitingDistance_Test(ITestOutputHelper output) : base(output)
        {
            LimitingDistanceCalculator = new CalculateLimitingDistance();
        }

        [Theory]
        [InlineData(.01, .01, true)]
        [InlineData(.012, .011, true)]
        [InlineData(.011, .012, true)]
        [InlineData(.02, .015, true)]
        [InlineData(.02, .014, false)]
        public void TestDeterminTreeInOrOut(decimal slopeDistance, decimal limitingDistance, bool expectedResult)
        {
            LimitingDistanceCalculator.DeterminTreeInOrOut(slopeDistance, limitingDistance).Should().Be(expectedResult);
        }

        [Theory]
        //variable radius
        [InlineData(20.0, 10.0, 0, true, true, 19.028)]//to face
        [InlineData(20.0, 10.0, 50, true, true, 21.274)]//to face, w/ 50% slope
        [InlineData(20.0, 10.0, 0, true, false, 19.445)]//to center
        //fixed
        [InlineData(50.0, 10.0, 0, false, true, 16.236)]//to face
        [InlineData(50.0, 10.0, 50, false, true, 18.152)]//to face, w/ 50% slope
        [InlineData(50.0, 10.0, 0, false, false, 16.653)]//to center
        public void TestCalculateLimitingDistance(decimal BAForFPS, decimal dbh, int slopePCT, bool isVar, bool isFace, decimal expected)
        {
            int sigDec = 3;

            var baf = (isVar) ? BAForFPS : 0.0m;
            var fps = (!isVar) ? BAForFPS : 0.0m;

            var ld = LimitingDistanceCalculator.Calculate(baf, fps, dbh, slopePCT, isVar, isFace);
            ld = Math.Round(ld, sigDec);
            expected = Math.Round(expected, 3);
            ld.Should().Be(expected);
        }
    }
}