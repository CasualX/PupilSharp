using System;
using System.Globalization;

namespace PupilReply
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var interactive = Environment.UserInteractive;

            if (interactive)
            {
                Console.WriteLine("Welcome to Pupil, the expression evaluator!");
                if (args.Length <= 0)
                {
                    Console.Write(@"
Enter an expression, eg. 2 + 3, and press enter.
Press ctrl - C to exit.

Built -in functions:
  +-*/^   : Operators with correct precedence.
  (expr)  : Group expression with parentheses.
  ans: Use answer from previous expression.
  pi, tau: Trigonometric constants.
  e: Euler's number.
  add, sub, mul, div, rem, pow, floor, ceil, round,
  abs, sqr, cube, sqrt, cbrt, min, max, gamma, fac,
  exp, expm1, ln, log, log2, log10, ln1p,
  mean, median, range, var, stdev,
  deg, rad, sin, cos, tan, asin, acos, atan, atan2,
  sinh, cosh, tanh
          : Use parens to provide arguments.
");
                }
            }

            var env = new Pupil.Environment();
            env.Variables["ans"] = 0.0;

            // Eval the  command line args
            if (args.Length > 0)
            {
                try
                {
                    var expr = new Pupil.Expression(env);
                    foreach (string arg in args)
                    {
                        expr.Feed(arg);
                    }
                    var result = expr.Result();
                    env.Variables["ans"] = result;
                    Console.WriteLine($"Ok: {result.ToString(CultureInfo.InvariantCulture)}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Err: {ex.Message}!");
                }
            }
            else
            {
                while (true)
                {
                    if (interactive)
                    {
                        Console.Write(">>> ");
                    }

                    var line = Console.ReadLine().Trim();
                    if (line.Length > 0)
                    {
                        try
                        {
                            var result = Pupil.Expression.Evaluate(env, line);
                            env.Variables["ans"] = result;
                            Console.WriteLine(result.ToString(CultureInfo.InvariantCulture));
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Erro: {ex.Message}!");
                        }
                    }
                }
            }
        }
    }
}
