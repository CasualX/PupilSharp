using System;
using System.Linq;

namespace Pupil
{
    public static class Builtins
    {
        #region Operators
        public static double Id(Environment env, ArraySegment<double> args)
        {
            if (args.Count == 1)
                return args.Array[args.Offset];
            throw new ArgumentException("bad argument");
        }
        public static double Add(Environment env, ArraySegment<double> args)
        {
            double result = 0.0;
            for (int i = args.Offset; i < args.Offset + args.Count; ++i)
                result += args.Array[i];
            return result;
        }
        public static double Sub(Environment env, ArraySegment<double> args)
        {
            if (args.Count == 1)
                return -args.Array[args.Offset];
            else if (args.Count == 2)
                return args.Array[args.Offset] - args.Array[args.Offset + 1];
            else
                throw new ArgumentException("bad argument");
        }
        public static double Mul(Environment env, ArraySegment<double> args)
        {
            double result = 1.0;
            for (int i = args.Offset; i < args.Offset + args.Count; ++i)
                result *= args.Array[i];
            return result;
        }
        public static double Div(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException("bad argument");
            return args.Array[args.Offset] / args.Array[args.Offset + 1];
        }
        public static double Rem(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException("bad argument");
            return args.Array[args.Offset] % args.Array[args.Offset + 1];
        }
        public static double Pow(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException("bad argument");
            return Math.Pow(args.Array[args.Offset], args.Array[args.Offset + 1]);
        }
        #endregion

        #region Operations
        public static double Floor(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Floor(args.Array[args.Offset]);
        }
        public static double Ceil(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Ceiling(args.Array[args.Offset]);
        }
        public static double Round(Environment env, ArraySegment<double> args)
        {
            if (args.Count == 1)
                return Math.Round(args.Array[args.Offset]);
            if (args.Count == 2)
                return Math.Round(args.Array[args.Offset], (int)args.Array[args.Offset + 1]);
            throw new ArgumentException("bad argument");
        }
        public static double Abs(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Abs(args.Array[args.Offset]);
        }
        public static double Sqr(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            double v = args.Array[args.Offset];
            return v * v;
        }
        public static double Cube(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            double v = args.Array[args.Offset];
            return v * v * v;
        }
        public static double Sqrt(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Sqrt(args.Array[args.Offset]);
        }
        public static double Cbrt(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Pow(args.Array[args.Offset], 1.0 / 3.0);
        }
        public static double Min(Environment env, ArraySegment<double> args)
        {
            if (args.Count <= 0)
                throw new ArgumentException("bad argument");
            return args.Min();
        }
        public static double Max(Environment env, ArraySegment<double> args)
        {
            if (args.Count <= 0)
                throw new ArgumentException("bad argument");
            return args.Max();
        }
        #endregion

        #region Transcendental
        public static double Exp(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Exp(args.Array[args.Offset]);
        }
        public static double Exp2(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Pow(2.0, args.Array[args.Offset]);
        }
        public static double Expm1(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            double x = args.Array[args.Offset];
            if (Math.Abs(x) < 1e-5)
                return x + 0.5 * x * x;
            else
                return Math.Exp(x) - 1.0;
        }
        public static double Log(Environment env, ArraySegment<double> args)
        {
            double log_base;
            if (args.Count == 1)
                log_base = Math.E;
            else if (args.Count == 2)
                log_base = args.Array[args.Offset + 1];
            else
                throw new ArgumentException("bad argument");
            return Math.Log(args.Array[args.Offset], log_base);
        }
        public static double E(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 0)
                throw new ArgumentException("bad argument");
            return Math.E;
        }
        #endregion

        #region Statistics
        public static double Mean(Environment env, ArraySegment<double> args)
        {
            return Add(env, args) / args.Count;
        }
        public static double Median(Environment env, ArraySegment<double> args)
        {
            if (args.Count <= 0)
                throw new ArgumentException("bad argument");
            var sorted = args.ToList();
            sorted.Sort();
            int c = args.Count / 2;
            if ((c & 1) == 0)
                return (sorted[c - 1] + sorted[c]) / 2.0;
            else
                return sorted[c];
        }
        public static double Range(Environment env, ArraySegment<double> args)
        {
            if (args.Count <= 0)
                throw new ArgumentException("bad argument");
            double min = args.Min();
            double max = args.Max();
            return max - min;
        }
        public static double Variance(Environment env, ArraySegment<double> args)
        {
            double mean = Mean(env, args);
            double acc = 0.0;
            for (int i = args.Offset; i < args.Offset + args.Count; ++i)
                acc += (args.Array[i] - mean) * (args.Array[i] - mean);
            return acc;
        }
        public static double Stddev(Environment env, ArraySegment<double> args)
        {
            return Math.Sqrt(Variance(env, args));
        }
        #endregion

        #region Trigonometry
        public static double Degrees(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return args.Array[args.Offset] * (180.0 / Math.PI);
        }
        public static double Radians(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return args.Array[args.Offset] * (Math.PI / 180.0);
        }
        public static double PI(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 0)
                throw new ArgumentException("bad argument");
            return Math.PI;
        }
        public static double Tau(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 0)
                throw new ArgumentException("bad argument");
            return Math.PI + Math.PI;
        }
        public static double Sin(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Sin(args.Array[args.Offset]);
        }
        public static double Cos(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Cos(args.Array[args.Offset]);
        }
        public static double Tan(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Tan(args.Array[args.Offset]);
        }
        public static double Asin(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Asin(args.Array[args.Offset]);
        }
        public static double Acos(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Acos(args.Array[args.Offset]);
        }
        public static double Atan(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Atan(args.Array[args.Offset]);
        }
        public static double Atan2(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 2)
                throw new ArgumentException("bad argument");
            return Math.Atan2(args.Array[args.Offset], args.Array[args.Offset + 1]);
        }
        public static double Sinh(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Sinh(args.Array[args.Offset]);
        }
        public static double Cosh(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Cosh(args.Array[args.Offset]);
        }
        public static double Tanh(Environment env, ArraySegment<double> args)
        {
            if (args.Count != 1)
                throw new ArgumentException("bad argument");
            return Math.Tanh(args.Array[args.Offset]);
        }
        #endregion
    }
}
