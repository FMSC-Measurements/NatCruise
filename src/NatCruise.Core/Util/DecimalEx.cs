using System;
using System.Diagnostics;
using System.Linq;

namespace NatCruise.Util
{


    /// <summary>
    /// Contains mathematical operations performed in Decimal precision.
    /// </summary>
    public static partial class DecimalMath
    {
        /// <summary> The pi (π) constant. Pi radians is equivalent to 180 degrees. </summary>
        /// <remarks> See http://en.wikipedia.org/wiki/Pi </remarks>
        public const decimal Pi = 3.1415926535897932384626433833m;              // 180 degrees - see http://en.wikipedia.org/wiki/Pi
        /// <summary> π/2 - in radians is equivalent to 90 degrees. </summary> 
        public const decimal PiHalf = 1.5707963267948966192313216916m;          //  90 degrees
        /// <summary> π/4 - in radians is equivalent to 45 degrees. </summary>
        public const decimal PiQuarter = 0.7853981633974483096156608458m;       //  45 degrees
        /// <summary> π/12 - in radians is equivalent to 15 degrees. </summary>
        public const decimal PiTwelfth = 0.2617993877991494365385536153m;       //  15 degrees
        /// <summary> 2π - in radians is equivalent to 360 degrees. </summary>
        public const decimal TwoPi = 6.2831853071795864769252867666m;           // 360 degrees

        /// <summary>
        /// Smallest non-zero decimal value.
        /// </summary>
        public const decimal SmallestNonZeroDec = 0.0000000000000000000000000001m; // aka new decimal(1, 0, 0, false, 28); //1e-28m

        /// <summary>
        /// The e constant, also known as "Euler's number" or "Napier's constant"
        /// </summary>
        /// <remarks>
        /// Full value is 2.718281828459045235360287471352662497757, 
        /// see http://mathworld.wolfram.com/e.html
        /// </remarks>
        public const decimal E = 2.7182818284590452353602874714m;

        /// <summary>
        /// The value of the natural logarithm of 10.
        /// </summary>
        /// <remarks>
        /// Full value is: 2.30258509299404568401799145468436420760110148862877297603332790096757
        /// From: http://oeis.org/A002392/constant
        /// </remarks>
        public const decimal Ln10 = 2.3025850929940456840179914547m;

        // Fast access for 10^n
        internal static readonly decimal[] PowersOf10 =
        {
            1m,
            10m,
            100m,
            1000m,
            10000m,
            100000m,
            1000000m,
            10000000m,
            100000000m,
            1000000000m,
            10000000000m,
            100000000000m,
            1000000000000m,
            10000000000000m,
            100000000000000m,
            1000000000000000m,
            10000000000000000m,
            100000000000000000m,
            1000000000000000000m,
            10000000000000000000m,
            100000000000000000000m,
            1000000000000000000000m,
            10000000000000000000000m,
            100000000000000000000000m,
            1000000000000000000000000m,
            10000000000000000000000000m,
            100000000000000000000000000m,
            1000000000000000000000000000m,
            10000000000000000000000000000m,
        };

        /// <summary>
        /// Returns the square root of a given number. 
        /// </summary>
        /// <param name="s">A non-negative number.</param>
        /// <remarks> 
        /// Uses an implementation of the "Babylonian Method".
        /// See http://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Babylonian_method 
        /// </remarks>
        public static decimal Sqrt(decimal s)
        {
            if (s < 0)
                throw new ArgumentException("Square root not defined for Decimal data type when less than zero!", "s");

            // Prevent divide-by-zero errors below. Dividing either
            // of the numbers below will yield a recurring 0 value
            // for halfS eventually converging on zero.
            if (s == 0 || s == SmallestNonZeroDec) return 0;

            decimal x;
            var halfS = s / 2m;
            var lastX = -1m;
            decimal nextX;

            // Begin with an estimate for the square root.
            // Use hardware to get us there quickly.
            x = (decimal)Math.Sqrt(decimal.ToDouble(s));

            while (true)
            {
                nextX = x / 2m + halfS / x;

                // The next check effectively sees if we've ran out of
                // precision for our data type.
                if (nextX == x || nextX == lastX) break;

                lastX = x;
                x = nextX;
            }

            return nextX;
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        /// <param name="x">A number to be raised to a power.</param>
        /// <param name="y">A number that specifies a power.</param>
        public static decimal Pow(decimal x, decimal y)
        {
            decimal result;
            var isNegativeExponent = false;

            // Handle negative exponents
            if (y < 0)
            {
                isNegativeExponent = true;
                y = Math.Abs(y);
            }

            if (y == 0)
            {
                result = 1;
            }
            else if (y == 1)
            {
                result = x;
            }
            else
            {
                var t = decimal.Truncate(y);

                if (y == t) // Integer powers
                {
                    result = ExpBySquaring(x, y);
                }
                else // Fractional power < 1
                {
                    // See http://en.wikipedia.org/wiki/Exponent#Real_powers
                    // The next line is an optimization of Exp(y * Log(x)) for better precision
                    result = ExpBySquaring(x, t) * Exp((y - t) * Log(x));
                }
            }

            if (isNegativeExponent)
            {
                // Note, for IEEE floats this would be Infinity and not an exception...
                if (result == 0) throw new OverflowException("Negative power of 0 is undefined!");

                result = 1 / result;
            }

            return result;
        }

        /// <summary>
        /// Raises one number to an integral power.
        /// </summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Exponentiation_by_squaring
        /// </remarks>
        private static decimal ExpBySquaring(decimal x, decimal y)
        {
            Debug.Assert(y >= 0 && decimal.Truncate(y) == y, "Only non-negative, integer powers supported.");
            if (y < 0) throw new ArgumentOutOfRangeException("y", "Negative exponents not supported!");
            if (decimal.Truncate(y) != y) throw new ArgumentException("Exponent must be an integer!", "y");

            var result = 1m;
            var multiplier = x;

            while (y > 0)
            {
                if ((y % 2) == 1)
                {
                    result *= multiplier;
                    y -= 1;
                    if (y == 0) break;
                }

                multiplier *= multiplier;
                y /= 2;
            }

            return result;
        }

        /// <summary>
        /// Returns e raised to the specified power.
        /// </summary>
        /// <param name="d">A number specifying a power.</param>
        public static decimal Exp(decimal d)
        {
            decimal result;
            decimal nextAdd;
            int iteration;
            bool reciprocal;
            decimal t;

            reciprocal = d < 0;
            d = Math.Abs(d);

            t = decimal.Truncate(d);

            if (d == 0)
            {
                result = 1;
            }
            else if (d == 1)
            {
                result = E;
            }
            else if (Math.Abs(d) > 1 && t != d)
            {
                // Split up into integer and fractional
                result = Exp(t) * Exp(d - t);
            }
            else if (d == t)   // Integer power
            {
                result = ExpBySquaring(E, d);
            }
            else                // Fractional power < 1
            {
                // See http://mathworld.wolfram.com/ExponentialFunction.html
                iteration = 0;
                nextAdd = 0;
                result = 0;

                while (true)
                {
                    if (iteration == 0)
                    {
                        nextAdd = 1;               // == Pow(d, 0) / Factorial(0) == 1 / 1 == 1
                    }
                    else
                    {
                        nextAdd *= d / iteration;  // == Pow(d, iteration) / Factorial(iteration)
                    }

                    if (nextAdd == 0) break;

                    result += nextAdd;

                    iteration += 1;
                }
            }

            // Take reciprocal if this was a negative power
            // Note that result will never be zero at this point.
            if (reciprocal) result = 1 / result;

            return result;
        }

        /// <summary>
        /// Returns the natural (base e) logarithm of a specified number.
        /// </summary>
        /// <param name="d">A number whose logarithm is to be found.</param>
        /// <remarks>
        /// I'm still not satisfied with the speed. I tried several different
        /// algorithms that you can find in a historical version of this 
        /// source file. The one I settled on was the best of mediocrity.
        /// </remarks>
        public static decimal Log(decimal d)
        {
            if (d < 0) throw new ArgumentException("Natural logarithm is a complex number for values less than zero!", "d");
            if (d == 0) throw new OverflowException("Natural logarithm is defined as negative infinity at zero which the Decimal data type can't represent!");

            if (d == 1) return 0;

            if (d >= 1)
            {
                var power = 0m;

                var x = d;
                while (x > 1)
                {
                    x /= 10;
                    power += 1;
                }

                return Log(x) + power * Ln10;
            }

            // See http://en.wikipedia.org/wiki/Natural_logarithm#Numerical_value
            // for more information on this faster-converging series.

            decimal y;
            decimal ySquared;

            var iteration = 0;
            var exponent = 0m;
            var nextAdd = 0m;
            var result = 0m;

            y = (d - 1) / (d + 1);
            ySquared = y * y;

            while (true)
            {
                if (iteration == 0)
                {
                    exponent = 2 * y;
                }
                else
                {
                    exponent = exponent * ySquared;
                }

                nextAdd = exponent / (2 * iteration + 1);

                if (nextAdd == 0) break;

                result += nextAdd;

                iteration += 1;
            }

            return result;

        }

        /// <summary>
        /// Returns the logarithm of a specified number in a specified base.
        /// </summary>
        /// <param name="d">A number whose logarithm is to be found.</param>
        /// <param name="newBase">The base of the logarithm.</param>
        /// <remarks>
        /// This is a relatively naive implementation that simply divides the
        /// natural log of <paramref name="d"/> by the natural log of the base.
        /// </remarks>
        public static decimal Log(decimal d, decimal newBase)
        {
            // Short circuit the checks below if d is 1 because
            // that will yield 0 in the numerator below and give us
            // 0 for any base, even ones that would yield infinity.
            if (d == 1) return 0m;

            if (newBase == 1) throw new InvalidOperationException("Logarithm for base 1 is undefined.");
            if (d < 0) throw new ArgumentException("Logarithm is a complex number for values less than zero!", nameof(d));
            if (d == 0) throw new OverflowException("Logarithm is defined as negative infinity at zero which the Decimal data type can't represent!");
            if (newBase < 0) throw new ArgumentException("Logarithm base would be a complex number for values less than zero!", nameof(newBase));
            if (newBase == 0) throw new OverflowException("Logarithm base would be negative infinity at zero which the Decimal data type can't represent!");

            return Log(d) / Log(newBase);
        }

        /// <summary>
        /// Gets the number of decimal places in a decimal value.
        /// </summary>
        /// <remarks>
        /// Started with something found here: http://stackoverflow.com/a/6092298/856595
        /// </remarks>
        public static int GetDecimalPlaces(decimal dec, bool countTrailingZeros)
        {
            const int signMask = unchecked((int)0x80000000);
            const int scaleMask = 0x00FF0000;
            const int scaleShift = 16;

            int[] bits = Decimal.GetBits(dec);
            var result = (bits[3] & scaleMask) >> scaleShift;  // extract exponent

            // Return immediately for values without a fractional portion or if we're counting trailing zeros
            if (countTrailingZeros || (result == 0)) return result;

            // Get a raw version of the decimal's integer
            bits[3] = bits[3] & ~unchecked(signMask | scaleMask); // clear out exponent and negative bit
            var rawValue = new decimal(bits);

            // Account for trailing zeros
            while ((result > 0) && ((rawValue % 10) == 0))
            {
                result--;
                rawValue /= 10;
            }

            return result;
        }

        /// <summary>
        /// Gets the remainder of one number divided by another number in such a way as to retain maximum precision.
        /// </summary>
        public static decimal Remainder(decimal d1, decimal d2)
        {
            if (Math.Abs(d1) < Math.Abs(d2)) return d1;

            var timesInto = decimal.Truncate(d1 / d2);
            var shiftingNumber = d2;
            var sign = Math.Sign(d1);

            for (var i = 0; i <= GetDecimalPlaces(d2, true); i++)
            {
                // Note that first "digit" will be the integer portion of d2
                var digit = decimal.Truncate(shiftingNumber);

                d1 -= timesInto * (digit / PowersOf10[i]);

                shiftingNumber = (shiftingNumber - digit) * 10m; // remove used digit and shift for next iteration
                if (shiftingNumber == 0m) break;
            }

            // If we've crossed zero because of the precision mismatch, 
            // we need to add a whole d2 to get a correct result.
            if (d1 != 0 && Math.Sign(d1) != sign)
            {
                d1 = Math.Sign(d2) == sign
                         ? d1 + d2
                         : d1 - d2;
            }

            return d1;
        }


        /// <summary>
        /// Returns the sine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <remarks>
        /// Uses a Taylor series to calculate sine. See 
        /// http://en.wikipedia.org/wiki/Trigonometric_functions for details.
        /// </remarks>
        public static decimal Sin(decimal x)
        {
            // Normalize to between -2Pi <= x <= 2Pi
            x = Remainder(x, TwoPi);

            if (x == 0 || x == Pi || x == TwoPi)
            {
                return 0;
            }
            if (x == PiHalf)
            {
                return 1;
            }
            if (x == Pi + PiHalf)
            {
                return -1;
            }

            var result = 0m;
            var doubleIteration = 0; // current iteration * 2
            var xSquared = x * x;
            var nextAdd = 0m;

            while (true)
            {
                if (doubleIteration == 0)
                {
                    nextAdd = x;
                }
                else
                {
                    // We multiply by -1 each time so that the sign of the component
                    // changes each time. The first item is positive and it
                    // alternates back and forth after that.
                    // Following is equivalent to: nextAdd *= -1 * x * x / ((2 * iteration) * (2 * iteration + 1));
                    nextAdd *= -1 * xSquared / (doubleIteration * doubleIteration + doubleIteration);
                }

#if DEBUGWITHMESSAGES
                Debug.WriteLine("{0:000}:{1,33:+0.0000000000000000000000000000;-0.0000000000000000000000000000} ->{2,33:+0.0000000000000000000000000000;-0.0000000000000000000000000000}",
                    doubleIteration / 2, nextAdd, result + nextAdd);
#endif
                if (nextAdd == 0) break;

                result += nextAdd;

                doubleIteration += 2;
            }

            return result;
        }

        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <remarks>
        /// Uses a Taylor series to calculate sine. See 
        /// http://en.wikipedia.org/wiki/Trigonometric_functions for details.
        /// </remarks>
        public static decimal Cos(decimal x)
        {
            // Normalize to between -2Pi <= x <= 2Pi
            x = Remainder(x, TwoPi);

            if (x == 0 || x == TwoPi)
            {
                return 1;
            }
            if (x == Pi)
            {
                return -1;
            }
            if (x == PiHalf || x == Pi + PiHalf)
            {
                return 0;
            }

            var result = 0m;
            var doubleIteration = 0; // current iteration * 2
            var xSquared = x * x;
            var nextAdd = 0m;

            while (true)
            {
                if (doubleIteration == 0)
                {
                    nextAdd = 1;
                }
                else
                {
                    // We multiply by -1 each time so that the sign of the component
                    // changes each time. The first item is positive and it
                    // alternates back and forth after that.
                    // Following is equivalent to: nextAdd *= -1 * x * x / ((2 * iteration - 1) * (2 * iteration));
                    nextAdd *= -1 * xSquared / (doubleIteration * doubleIteration - doubleIteration);
                }

                if (nextAdd == 0) break;

                result += nextAdd;

                doubleIteration += 2;
            }

            return result;
        }

        /// <summary>
        /// Returns the angle whose tangent is the quotient of two specified numbers.
        /// </summary>
        /// <param name="x">A number representing a tangent.</param>
        /// <remarks>
        /// See http://mathworld.wolfram.com/InverseTangent.html for faster converging 
        /// series from Euler that was used here.
        /// </remarks>
        public static decimal ATan(decimal x)
        {
            // Special cases
            if (x == -1) return -PiQuarter;
            if (x == 0) return 0;
            if (x == 1) return PiQuarter;
            if (x < -1)
            {
                // Force down to -1 to 1 interval for faster convergence
                return -PiHalf - ATan(1 / x);
            }
            if (x > 1)
            {
                // Force down to -1 to 1 interval for faster convergence
                return PiHalf - ATan(1 / x);
            }

            var result = 0m;
            var doubleIteration = 0; // current iteration * 2
            var y = (x * x) / (1 + x * x);
            var nextAdd = 0m;

            while (true)
            {
                if (doubleIteration == 0)
                {
                    nextAdd = x / (1 + x * x);  // is = y / x  but this is better for very small numbers where y = 9
                }
                else
                {
                    // We multiply by -1 each time so that the sign of the component
                    // changes each time. The first item is positive and it
                    // alternates back and forth after that.
                    // Following is equivalent to: nextAdd *= y * (iteration * 2) / (iteration * 2 + 1);
                    nextAdd *= y * doubleIteration / (doubleIteration + 1);
                }

                if (nextAdd == 0) break;

                result += nextAdd;

                doubleIteration += 2;
            }

            return result;
        }

        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        /// <param name="radians">An angle, measured in radians.</param>
        /// <remarks>
        /// Uses a Taylor series to calculate sine. See 
        /// http://en.wikipedia.org/wiki/Trigonometric_functions for details.
        /// </remarks>
        public static decimal Tan(decimal radians)
        {
            try
            {
                return Sin(radians) / Cos(radians);
            }
            catch (DivideByZeroException)
            {
                throw new Exception("Tangent is undefined at this angle!");
            }
        }
    }
}