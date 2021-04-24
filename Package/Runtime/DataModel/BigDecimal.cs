using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace HeathenEngineering.BGSDK.DataModel
{
    /// <summary>
    /// Based on BigFloat class from 
    /// https://github.com/Osinko/BigFloat/blob/master/src/BigFloat.cs
    /// </summary>
    [Serializable]
    public struct BigDecimal : IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal>
    {
        private BigInteger numerator;
        private BigInteger denominator;

        public static readonly BigDecimal One = new BigDecimal(1);
        public static readonly BigDecimal Zero = new BigDecimal(0);
        public static readonly BigDecimal MinusOne = new BigDecimal(-1);
        public static readonly BigDecimal OneHalf = new BigDecimal(1, 2);

        public int Sign
        {
            get
            {
                switch (numerator.Sign + denominator.Sign)
                {
                    case 2:
                    case -2:
                        return 1;
                    case 0:
                        return -1;
                    default:
                        return 0;
                }
            }
        }

        public BigDecimal(string value)
        {
            BigDecimal bf = Parse(value);
            this.numerator = bf.numerator;
            this.denominator = bf.denominator;
        }
        public BigDecimal(BigInteger numerator, BigInteger denominator)
        {
            this.numerator = numerator;
            if (denominator == 0)
                throw new ArgumentException("denominator equals 0");
            this.denominator = BigInteger.Abs(denominator);
        }
        public BigDecimal(BigInteger value)
        {
            this.numerator = value;
            this.denominator = BigInteger.One;
        }
        public BigDecimal(BigDecimal value)
        {
            if (BigDecimal.Equals(value, null))
            {
                this.numerator = BigInteger.Zero;
                this.denominator = BigInteger.One;
            }
            else
            {

                this.numerator = value.numerator;
                this.denominator = value.denominator;
            }
        }
        public BigDecimal(ulong value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigDecimal(long value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigDecimal(uint value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigDecimal(int value)
        {
            numerator = new BigInteger(value);
            denominator = BigInteger.One;
        }
        public BigDecimal(float value) : this(value.ToString("N99"))
        {
        }
        public BigDecimal(double value) : this(value.ToString("N99"))
        {
        }
        public BigDecimal(decimal value) : this(value.ToString("N99"))
        {
        }

        //non-static methods
        public BigDecimal Add(BigDecimal other)
        {
            if (BigDecimal.Equals(other, null))
                throw new ArgumentNullException("other");

            this.numerator = this.numerator * other.denominator + other.numerator * this.denominator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigDecimal Subtract(BigDecimal other)
        {
            if (BigDecimal.Equals(other, null))
                throw new ArgumentNullException("other");

            this.numerator = this.numerator * other.denominator - other.numerator * this.denominator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigDecimal Multiply(BigDecimal other)
        {
            if (BigDecimal.Equals(other, null))
                throw new ArgumentNullException("other");

            this.numerator *= other.numerator;
            this.denominator *= other.denominator;
            return this;
        }
        public BigDecimal Divide(BigDecimal other)
        {
            if (BigInteger.Equals(other, null))
                throw new ArgumentNullException("other");
            if (other.numerator == 0)
                throw new System.DivideByZeroException("other");

            this.numerator *= other.denominator;
            this.denominator *= other.numerator;
            return this;
        }
        public BigDecimal Remainder(BigDecimal other)
        {
            if (BigInteger.Equals(other, null))
                throw new ArgumentNullException("other");

            //b = a mod n
            //remainder = a - floor(a/n) * n

            BigDecimal result = this - Floor(this / other) * other;

            this.numerator = result.numerator;
            this.denominator = result.denominator;


            return this;
        }
        public BigDecimal DivideRemainder(BigDecimal other, out BigDecimal remainder)
        {
            this.Divide(other);

            remainder = BigDecimal.Remainder(this, other);

            return this;
        }
        public BigDecimal Pow(int exponent)
        {
            if (numerator.IsZero)
            {
                // Nothing to do
            }
            else if (exponent < 0)
            {
                BigInteger savedNumerator = numerator;
                numerator = BigInteger.Pow(denominator, -exponent);
                denominator = BigInteger.Pow(savedNumerator, -exponent);
            }
            else
            {
                numerator = BigInteger.Pow(numerator, exponent);
                denominator = BigInteger.Pow(denominator, exponent);
            }

            return this;
        }
        public BigDecimal Abs()
        {
            numerator = BigInteger.Abs(numerator);
            return this;
        }
        public BigDecimal Negate()
        {
            numerator = BigInteger.Negate(numerator);
            return this;
        }
        public BigDecimal Inverse()
        {
            BigInteger temp = numerator;
            numerator = denominator;
            denominator = temp;
            return this;
        }
        public BigDecimal Increment()
        {
            numerator += denominator;
            return this;
        }
        public BigDecimal Decrement()
        {
            numerator -= denominator;
            return this;
        }
        public BigDecimal Ceil()
        {
            if (numerator < 0)
                numerator -= BigInteger.Remainder(numerator, denominator);
            else
                numerator += denominator - BigInteger.Remainder(numerator, denominator);

            Factor();
            return this;
        }
        public BigDecimal Floor()
        {
            if (numerator < 0)
                numerator += denominator - BigInteger.Remainder(numerator, denominator);
            else
                numerator -= BigInteger.Remainder(numerator, denominator);

            Factor();
            return this;
        }
        public BigDecimal Round()
        {
            //get remainder. Over divisor see if it is > new BigFloat(0.5)
            BigDecimal value = BigDecimal.Decimals(this);

            if (value.CompareTo(OneHalf) >= 0)
                this.Ceil();
            else
                this.Floor();

            return this;
        }
        public BigDecimal Truncate()
        {
            numerator -= BigInteger.Remainder(numerator, denominator);
            Factor();
            return this;
        }
        public BigDecimal Decimals()
        {
            BigInteger result = BigInteger.Remainder(numerator, denominator);

            return new BigDecimal(result, denominator);
        }
        public BigDecimal ShiftDecimalLeft(int shift)
        {
            if (shift < 0)
                return ShiftDecimalRight(-shift);

            numerator *= BigInteger.Pow(10, shift);
            return this;
        }
        public BigDecimal ShiftDecimalRight(int shift)
        {
            if (shift < 0)
                return ShiftDecimalLeft(-shift);
            denominator *= BigInteger.Pow(10, shift);
            return this;
        }
        public double Sqrt()
        {
            return Math.Pow(10, BigInteger.Log10(numerator) / 2) / Math.Pow(10, BigInteger.Log10(denominator) / 2);
        }
        public double Log10()
        {
            return BigInteger.Log10(numerator) - BigInteger.Log10(denominator);
        }
        public double Log(double baseValue)
        {
            return BigInteger.Log(numerator, baseValue) - BigInteger.Log(numerator, baseValue);
        }
        public override string ToString()
        {
            //default precision = 100
            return ToString(100);
        }
        public string ToString(int precision, bool trailingZeros = false)
        {
            Factor();

            BigInteger remainder;
            BigInteger result = BigInteger.DivRem(numerator, denominator, out remainder);

            if (remainder == 0 && trailingZeros)
                return result + ".0";
            else if (remainder == 0)
                return result.ToString();


            BigInteger decimals = (numerator * BigInteger.Pow(10, precision)) / denominator;

            if (decimals == 0 && trailingZeros)
                return result + ".0";
            else if (decimals == 0)
                return result.ToString();

            StringBuilder sb = new StringBuilder();

            while (precision-- > 0 && decimals > 0)
            {
                sb.Append(decimals % 10);
                decimals /= 10;
            }

            if (trailingZeros)
                return result + "." + new string(sb.ToString().Reverse().ToArray());
            else
                return result + "." + new string(sb.ToString().Reverse().ToArray()).TrimEnd(new char[] { '0' });


        }
        public string ToMixString()
        {
            Factor();

            BigInteger remainder;
            BigInteger result = BigInteger.DivRem(numerator, denominator, out remainder);

            if (remainder == 0)
                return result.ToString();
            else
                return result + ", " + remainder + "/" + denominator;
        }

        public string ToRationalString()
        {
            Factor();

            return numerator + " / " + denominator;
        }
        public int CompareTo(BigDecimal other)
        {
            if (BigDecimal.Equals(other, null))
                throw new ArgumentNullException("other");

            //Make copies
            BigInteger one = this.numerator;
            BigInteger two = other.numerator;

            //cross multiply
            one *= other.denominator;
            two *= this.denominator;

            //test
            return BigInteger.Compare(one, two);
        }
        public int CompareTo(object other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if (!(other is BigDecimal))
                throw new System.ArgumentException("other is not a BigFloat");

            return CompareTo((BigDecimal)other);
        }
        public override bool Equals(object other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            return this.numerator == ((BigDecimal)other).numerator && this.denominator == ((BigDecimal)other).denominator;
        }
        public bool Equals(BigDecimal other)
        {
            return (other.numerator == this.numerator && other.denominator == this.denominator);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //static methods
        public new static bool Equals(object left, object right)
        {
            if (left == null && right == null) return true;
            else if (left == null || right == null) return false;
            else if (left.GetType() != right.GetType()) return false;
            else
                return (((BigInteger)left).Equals((BigInteger)right));
        }
        public static string ToString(BigDecimal value)
        {
            return value.ToString();
        }

        public static BigDecimal Inverse(BigDecimal value)
        {
            return (new BigDecimal(value)).Inverse();
        }
        public static BigDecimal Decrement(BigDecimal value)
        {
            return (new BigDecimal(value)).Decrement();
        }
        public static BigDecimal Negate(BigDecimal value)
        {
            return (new BigDecimal(value)).Negate();
        }
        public static BigDecimal Increment(BigDecimal value)
        {
            return (new BigDecimal(value)).Increment();
        }
        public static BigDecimal Abs(BigDecimal value)
        {
            return (new BigDecimal(value)).Abs();
        }
        public static BigDecimal Add(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Add(right);
        }
        public static BigDecimal Subtract(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Subtract(right);
        }
        public static BigDecimal Multiply(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Multiply(right);
        }
        public static BigDecimal Divide(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Divide(right);
        }
        public static BigDecimal Pow(BigDecimal value, int exponent)
        {
            return (new BigDecimal(value)).Pow(exponent);
        }
        public static BigDecimal Remainder(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Remainder(right);
        }
        public static BigDecimal DivideRemainder(BigDecimal left, BigDecimal right, out BigDecimal remainder)
        {
            return (new BigDecimal(left)).DivideRemainder(right, out remainder);
        }
        public static BigDecimal Decimals(BigDecimal value)
        {
            return value.Decimals();
        }
        public static BigDecimal Truncate(BigDecimal value)
        {
            return (new BigDecimal(value)).Truncate();
        }
        public static BigDecimal Ceil(BigDecimal value)
        {
            return (new BigDecimal(value)).Ceil();
        }
        public static BigDecimal Floor(BigDecimal value)
        {
            return (new BigDecimal(value)).Floor();
        }
        public static BigDecimal Round(BigDecimal value)
        {
            return (new BigDecimal(value)).Round();
        }
        public static BigDecimal Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            value.Trim();
            value = value.Replace(",", "");
            int pos = value.IndexOf('.');
            value = value.Replace(".", "");

            if (pos < 0)
            {
                //no decimal point
                BigInteger numerator = BigInteger.Parse(value);
                return (new BigDecimal(numerator)).Factor();
            }
            else
            {
                //decimal point (length - pos - 1)
                BigInteger numerator = BigInteger.Parse(value);
                BigInteger denominator = BigInteger.Pow(10, value.Length - pos);

                return (new BigDecimal(numerator, denominator)).Factor();
            }
        }
        public static BigDecimal ShiftDecimalLeft(BigDecimal value, int shift)
        {
            return (new BigDecimal(value)).ShiftDecimalLeft(shift);
        }
        public static BigDecimal ShiftDecimalRight(BigDecimal value, int shift)
        {
            return (new BigDecimal(value)).ShiftDecimalRight(shift);
        }
        public static bool TryParse(string value, out BigDecimal result)
        {
            try
            {
                result = BigDecimal.Parse(value);
                return true;
            }
            catch (ArgumentNullException)
            {
                result = new BigDecimal();
                return false;
            }
            catch (FormatException)
            {
                result = new BigDecimal();
                return false;
            }
        }
        public static int Compare(BigDecimal left, BigDecimal right)
        {
            if (BigDecimal.Equals(left, null))
                throw new ArgumentNullException("left");
            if (BigDecimal.Equals(right, null))
                throw new ArgumentNullException("right");

            return (new BigDecimal(left)).CompareTo(right);
        }
        public static double Log10(BigDecimal value)
        {
            return (new BigDecimal(value)).Log10();
        }
        public static double Log(BigDecimal value, double baseValue)
        {
            return (new BigDecimal(value)).Log(baseValue);
        }
        public static double Sqrt(BigDecimal value)
        {
            return (new BigDecimal(value)).Sqrt();
        }

        public static BigDecimal operator -(BigDecimal value)
        {
            return (new BigDecimal(value)).Negate();
        }
        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Subtract(right);
        }
        public static BigDecimal operator --(BigDecimal value)
        {
            return value.Decrement();
        }
        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Add(right);
        }
        public static BigDecimal operator +(BigDecimal value)
        {
            return (new BigDecimal(value)).Abs();
        }
        public static BigDecimal operator ++(BigDecimal value)
        {
            return value.Increment();
        }
        public static BigDecimal operator %(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Remainder(right);
        }
        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Multiply(right);
        }
        public static BigDecimal operator /(BigDecimal left, BigDecimal right)
        {
            return (new BigDecimal(left)).Divide(right);
        }
        public static BigDecimal operator >>(BigDecimal value, int shift)
        {
            return (new BigDecimal(value)).ShiftDecimalRight(shift);
        }
        public static BigDecimal operator <<(BigDecimal value, int shift)
        {
            return (new BigDecimal(value)).ShiftDecimalLeft(shift);
        }
        public static BigDecimal operator ^(BigDecimal left, int right)
        {
            return (new BigDecimal(left)).Pow(right);
        }
        public static BigDecimal operator ~(BigDecimal value)
        {
            return (new BigDecimal(value)).Inverse();
        }

        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) != 0;
        }
        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) == 0;
        }
        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) < 0;
        }
        public static bool operator <=(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) <= 0;
        }
        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) > 0;
        }
        public static bool operator >=(BigDecimal left, BigDecimal right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator true(BigDecimal value)
        {
            return value != 0;
        }
        public static bool operator false(BigDecimal value)
        {
            return value == 0;
        }

        public static explicit operator decimal(BigDecimal value)
        {
            if (decimal.MinValue > value) throw new System.OverflowException("value is less than System.decimal.MinValue.");
            if (decimal.MaxValue < value) throw new System.OverflowException("value is greater than System.decimal.MaxValue.");

            return (decimal)value.numerator / (decimal)value.denominator;
        }
        public static explicit operator double(BigDecimal value)
        {
            if (double.MinValue > value) throw new System.OverflowException("value is less than System.double.MinValue.");
            if (double.MaxValue < value) throw new System.OverflowException("value is greater than System.double.MaxValue.");

            return (double)value.numerator / (double)value.denominator;
        }
        public static explicit operator float(BigDecimal value)
        {
            if (float.MinValue > value) throw new System.OverflowException("value is less than System.float.MinValue.");
            if (float.MaxValue < value) throw new System.OverflowException("value is greater than System.float.MaxValue.");

            return (float)value.numerator / (float)value.denominator;
        }

        //byte, sbyte, 
        public static implicit operator BigDecimal(byte value)
        {
            return new BigDecimal((uint)value);
        }
        public static implicit operator BigDecimal(sbyte value)
        {
            return new BigDecimal((int)value);
        }
        public static implicit operator BigDecimal(short value)
        {
            return new BigDecimal((int)value);
        }
        public static implicit operator BigDecimal(ushort value)
        {
            return new BigDecimal((uint)value);
        }
        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(long value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(uint value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(ulong value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(decimal value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(double value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(float value)
        {
            return new BigDecimal(value);
        }
        public static implicit operator BigDecimal(BigInteger value)
        {
            return new BigDecimal(value);
        }
        public static explicit operator BigDecimal(string value)
        {
            return new BigDecimal(value);
        }

        private BigDecimal Factor()
        {
            //factoring can be very slow. So use only when neccessary (ToString, and comparisons)

            if (denominator == 1)
                return this;

            //factor numerator and denominator
            BigInteger factor = BigInteger.GreatestCommonDivisor(numerator, denominator);

            numerator /= factor;
            denominator /= factor;

            return this;
        }

    }
}
