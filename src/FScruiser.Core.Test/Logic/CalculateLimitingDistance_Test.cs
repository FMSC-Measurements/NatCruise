using FluentAssertions;
using FScruiser.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Logic
{
    public class CalculateLimitingDistance_Test : TestBase
    {
        public CalculateLimitingDistance_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(.01, .01, true)]
        [InlineData(.012, .011, true)]
        [InlineData(.011, .012, true)]
        [InlineData(.02, .015, true)]
        [InlineData(.02, .014, false)]
        //for tree to be in slope distance (first value) must be less than or equal to limiting distance (second value)
        //both values are round to two decimal places
        //here we test edge cases where both values should be determined to be equal
        public void TestDeterminTreeInOrOut(double slopeDistance, double limitingDistance, bool expectedResult)
        {
            CalculateLimitingDistance.DeterminTreeInOrOut(slopeDistance, limitingDistance).Should().Be(expectedResult);
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
        public void TestCalculateLimitingDistance(double BAForFPS, double dbh, int slopePCT, bool isVar, bool isFace, double expected)
        {
            int sigDec = 3;

            var ld = CalculateLimitingDistance.Calculate(BAForFPS, dbh, slopePCT, isVar, isFace);
            ld = Math.Round(ld, sigDec);
            expected = Math.Round(expected, 3);
            ld.Should().Be(expected);
        }
    }
}
