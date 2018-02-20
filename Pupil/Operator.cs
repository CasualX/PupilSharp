using System;

namespace Pupil
{
    /// <summary>
    /// Known arithmetic operator kinds.
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// Addition.
        /// </summary>
        Add,
        /// <summary>
        /// Subtraction.
        /// </summary>
        Sub,
        /// <summary>
        /// Multiplication
        /// </summary>
        Mul,
        /// <summary>
        /// Division.
        /// </summary>
        Div,
        /// <summary>
        /// Remainder.
        /// </summary>
        Rem,
        /// <summary>
        /// Implicit Multiplication.
        ///
        /// This Operator does not have symbol, instead it is generated as needed by the parser.
        /// </summary>
        IMul,
        /// <summary>
        /// Exponentiation.
        /// </summary>
        Exp,
    }

    /// <summary>
    /// Operator Associativity.
    /// 
    /// See <a href="https://en.wikipedia.org/wiki/Operator_associativity">wikipedia</a> for more information. 
    /// </summary>
    public enum Associativity
    {
        /// <summary>
        /// All <see cref="Operator"/> except exponentiation are left associative.
        /// </summary>
        Left,
        /// <summary>
        /// Exponentiation is the only known right associative operator.
        /// </summary>
        Right,
        /// <summary>
        /// Not implemented.
        /// </summary>
        None,
    }

    /// <summary>
    /// Generalization of <see cref="Operator"/> Precedence rules.
    /// 
    /// See <a href="https://en.wikipedia.org/wiki/Order_of_operations">wikipedia</a> for more information.
    /// </summary>
    public enum Order
    {
        /// <summary>
        /// Special Precedence to prevent function arguments to evaluate across function calls.
        /// </summary>
        FnBarrier,
        /// <summary>
        /// Addition and Subtraction Precedence.
        /// </summary>
        AddSub,
        /// <summary>
        /// Multiplication and Division Precedence.
        /// </summary>
        MulDiv,
        /// <summary>
        /// Special Precedence for <see cref="Operator.IMul"/> Implicit Multiplication to bind tighter than division.
        /// </summary>
        IMul,
        /// <summary>
        /// Unary operators have the highest Precedence.
        /// </summary>
        Unary,
        /// <summary>
        /// Exponentiation.
        /// </summary>
        Exp,
    }

    public static class OperatorExtensions
    {
        /// <summary>
        /// Returns the Precedence for the given <see cref="Operator"/>.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Order Precedence(this Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                case Operator.Sub:
                    return Order.AddSub;
                case Operator.Mul:
                case Operator.Div:
                case Operator.Rem:
                    return Order.MulDiv;
                case Operator.IMul:
                    return Order.IMul;
                case Operator.Exp:
                    return Order.Exp;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown Operator");
            }
        }
        /// <summary>
        /// Returns the Associativity for the given <see cref="Operator"/>.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Associativity Associativity(this Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                case Operator.Sub:
                case Operator.Mul:
                case Operator.Div:
                case Operator.Rem:
                case Operator.IMul:
                    return Pupil.Associativity.Left;
                case Operator.Exp:
                    return Pupil.Associativity.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown Operator");
            }
        }
        /// <summary>
        /// Returns whether the given <see cref="Operator"/> is allowed as a unary operator.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool Unary(this Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                case Operator.Sub:
                    return true;
                case Operator.Mul:
                case Operator.Div:
                case Operator.Rem:
                case Operator.IMul:
                case Operator.Exp:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown Operator");
            }
        }
        /// <summary>
        /// Returns the builtin function for the given <see cref="Operator"/>.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Func<Environment, ArraySegment<double>, double> Builtin(this Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                    return Builtins.Add;
                case Operator.Sub:
                    return Builtins.Sub;
                case Operator.Mul:
                    return Builtins.Mul;
                case Operator.Div:
                    return Builtins.Div;
                case Operator.Rem:
                    return Builtins.Rem;
                case Operator.IMul:
                    return Builtins.Mul;
                case Operator.Exp:
                    return Builtins.Pow;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown Operator");
            }
        }
    }
}
