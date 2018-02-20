using System;
using System.Collections.Generic;

namespace Pupil
{
    public class Environment
    {
        /// <summary>
        /// Variables available to the expression.
        /// </summary>
        public Dictionary<string, double> Variables { get; } = new Dictionary<string, double>();
        /// <summary>
        /// Builtin functions available to the expression.
        /// </summary>
        public Dictionary<string, Func<Environment, ArraySegment<double>, double>> Functions { get; } = new Dictionary<string, Func<Environment, ArraySegment<double>, double>>();

        public Environment()
        {
            Variables.Add("ans", 0.0);

            Functions.Add("", Builtins.Id);
            Functions.Add("add", Builtins.Add);
            Functions.Add("sub", Builtins.Sub);
            Functions.Add("mul", Builtins.Mul);
            Functions.Add("div", Builtins.Div);
            Functions.Add("rem", Builtins.Rem);
            Functions.Add("pow", Builtins.Pow);

            Functions.Add("floor", Builtins.Floor);
            Functions.Add("ceil", Builtins.Ceil);
            Functions.Add("round", Builtins.Round);
            Functions.Add("abs", Builtins.Abs);
            Functions.Add("sqr", Builtins.Sqr);
            Functions.Add("cube", Builtins.Cube);
            Functions.Add("sqrt", Builtins.Sqrt);
            Functions.Add("cbrt", Builtins.Cbrt);
            Functions.Add("min", Builtins.Min);
            Functions.Add("max", Builtins.Max);

            Functions.Add("exp", Builtins.Exp);
            Functions.Add("exp2", Builtins.Exp2);
            Functions.Add("expm1", Builtins.Expm1);
            Functions.Add("log", Builtins.Log);
            Functions.Add("e", Builtins.E);

            Functions.Add("mean", Builtins.Mean);
            Functions.Add("median", Builtins.Median);
            Functions.Add("range", Builtins.Range);
            Functions.Add("var", Builtins.Variance);
            Functions.Add("stddev", Builtins.Stddev);

            Functions.Add("deg", Builtins.Degrees);
            Functions.Add("rad", Builtins.Radians);
            Functions.Add("pi", Builtins.PI);
            Functions.Add("tau", Builtins.Tau);
            Functions.Add("sin", Builtins.Sin);
            Functions.Add("cos", Builtins.Cos);
            Functions.Add("tan", Builtins.Tan);
            Functions.Add("asin", Builtins.Asin);
            Functions.Add("acos", Builtins.Acos);
            Functions.Add("atan", Builtins.Atan);
            Functions.Add("atan2", Builtins.Atan2);
            Functions.Add("sinh", Builtins.Sinh);
            Functions.Add("cosh", Builtins.Cosh);
            Functions.Add("tanh", Builtins.Tanh);
        }
    }
}
