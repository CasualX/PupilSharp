using System;
using System.Collections.Generic;

namespace Pupil
{
    enum State
    {
        Result,
        Value,
        Operator,
    }
    struct FnVal
    {
        public Func<Environment, ArraySegment<double>, double> function;
        public Order order;
        public int nargs;
    }

    struct Stack<T> where T: struct
    {
        T[] array;
        int length;

        public T[] Array
        {
            get { return array; }
        }
        public int Length
        {
            get { return length; }
        }
        public int Capacity
        {
            get { return array?.Length ?? 0; }
        }

        public void Push(T value)
        {
            // Reallocate the array if needed
            if (array == null)
            {
                array = new T[32];
            }
            else if (length >= array.Length)
            {
                T[] new_array = new T[array.Length * 2];
                array.CopyTo(new_array, 0);
                array = new_array;
            }
            array[length++] = value;
        }
        public T? Pop()
        {
            if (length <= 0)
                return null;
            length -= 1;
            return array[length];
        }
        public ArraySegment<T>? Top(int n)
        {
            if (n > length)
                return null;
            length -= n;
            return new ArraySegment<T>(array, length, n);
        }
        public T? Peek()
        {
            if (length <= 0)
                return null;
            return array[length - 1];
        }
        public bool Poke(Func<T, T> f)
        {
            if (length <= 0)
                return false;
            array[length - 1] = f(array[length - 1]);
            return true;
        }
    }

    /// <summary>
    /// Evaluation context.
    /// </summary>
    public class Expression
    {
        Environment environment;
        Stack<FnVal> functions;
        Stack<double> values;
        State next;

        public Expression(Environment env)
        {
            environment = env;
            functions = new Stack<FnVal>();
            values = new Stack<double>();
            next = State.Value;
        }
        /// <summary>
        /// Parses the next token.
        /// </summary>
        /// <param name="token"></param>
        public void Parse(Token token)
        {
            switch (next)
            {
                case State.Operator:
                    ParseOperator(token);
                    break;
                case State.Value:
                    ParseValue(token);
                    break;
                case State.Result:
                    throw new InvalidOperationException("No new tokens can be parsed because the expression is finished, create a new expression to start over");
            }
        }
        /// <summary>
        /// Tokenize and parse the input string.
        /// </summary>
        /// <param name="input"></param>
        public void Feed(string input)
        {
            foreach (Token token in Lexer.Tokenize(input))
            {
                Parse(token);
            }
        }
        /// <summary>
        /// When all tokens are parsed, force evaluation and return the final result.
        /// </summary>
        /// <returns></returns>
        public double Result()
        {
            if (next == State.Value)
                throw new InvalidOperationException("The expression is unfinished because it ends with an operator");
            EvalGT(Order.FnBarrier);
            if (values.Length != 1 || functions.Length != 0)
                throw new InvalidOperationException("The expression contains unevaluated functions, add closing parentheses");
            next = State.Result;
            return values.Pop().Value;
        }
        /// <summary>
        /// Convenience helper to tokenize, parse and calculate the result of the expression.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Evaluate(Environment env, string input)
        {
            var expr = new Expression(env);
            expr.Feed(input);
            return expr.Result();
        }

        #region Implementation
        void ParseValue(Token token)
        {
            switch (token.kind)
            {
                case TokenKind.Unknown:
                    throw new InvalidOperationException($"Invalid token at position {token.value}");
                case TokenKind.Literal:
                    // Push all literals on the value stack
                    values.Push((double)token.value);
                    // Expect to be followed by an operator
                    next = State.Operator;
                    break;
                case TokenKind.Operator:
                    // Unary operators are allowed when a value is expected
                    Operator op = (Operator)token.value;
                    if (!op.Unary())
                        throw new InvalidOperationException($"Cannot use {op} as a unary operator");
                    // Push the operator with the unary precedence
                    functions.Push(new FnVal { function = op.Builtin(), order = Order.Unary, nargs = 1 });
                    // Expect to be followed by a value
                    next = State.Value;
                    break;
                case TokenKind.Variable:
                    // Similar to a literal, push on the value stack
                    string var_name = ((string)token.value).ToLowerInvariant();
                    double value;
                    if (environment.Functions.ContainsKey(var_name))
                        value = environment.Functions[var_name](environment, new ArraySegment<double>());
                    else if (environment.Variables.ContainsKey(var_name))
                        value = environment.Variables[var_name];
                    else
                        throw new InvalidOperationException($"The variable or constant {var_name} was not found in the environment");
                    values.Push(value);
                    // Expect to be followed by an operator
                    next = State.Operator;
                    break;
                case TokenKind.Function:
                    // Push a function with very low precedence to guard operator evaluation to not push past it
                    string fn_name = ((string)token.value).ToLowerInvariant();
                    var fn = environment.Functions[fn_name];
                    functions.Push(new FnVal { function = fn, order = Order.FnBarrier, nargs = 1 });
                    // Functions are followed by their arguments
                    next = State.Value;
                    break;
                case TokenKind.Comma:
                    throw new InvalidOperationException("Did not expect a comma in this position");
                case TokenKind.EndFunction:
                    throw new InvalidOperationException("Did not expect a closing parenthesis in this position");
                default:
                    throw new ArgumentOutOfRangeException(nameof(token.kind), token.kind, "Unknown TokenKind");
            }
        }
        void ParseOperator(Token token)
        {
            switch (token.kind)
            {
                case TokenKind.Unknown:
                    throw new InvalidOperationException($"Invalid token at position {token.value}");
                case TokenKind.Literal:
                    throw new InvalidOperationException($"Did not expect the literal {token.value} in this position");
                case TokenKind.Operator:
                    Operator op = (Operator)token.value;
                    // Evaluate higher precedence operators
                    Order pre = op.Precedence();
                    switch (op.Associativity())
                    {
                        case Associativity.Left:
                            EvalGE(pre);
                            break;
                        case Associativity.Right:
                            EvalGT(pre);
                            break;
                        case Associativity.None:
                        default:
                            throw new NotImplementedException();
                    }
                    // Push the operator function, these operators always expect two arguments
                    functions.Push(new FnVal { function = op.Builtin(), order = pre, nargs = 2 });
                    // Expect to be followed by a value
                    next = State.Value;
                    break;
                case TokenKind.Variable:
                case TokenKind.Function:
                    // Insert implicit multiplication token
                    ParseOperator(new Token
                    {
                        kind = TokenKind.Operator,
                        value = Operator.IMul,
                    });
                    // And retry this variable or function token
                    ParseValue(token);
                    break;
                case TokenKind.Comma:
                    // Evaluate the previous function argument fully before adding new arguments
                    EvalGT(Order.FnBarrier);
                    // Increase the number of arguments passed to the function
                    if (!functions.Poke(head => new FnVal { function = head.function, order = head.order, nargs = head.nargs + 1 }))
                        throw new InvalidOperationException($"Found a comma outside a function call");
                    // Expect to be followed by a value
                    next = State.Value;
                    break;
                case TokenKind.EndFunction:
                    // Evaluate the final function argument
                    EvalGT(Order.FnBarrier);
                    // Evaluate the function and push past the barrier
                    EvalApply();
                    // Expect to be followed by an operator
                    next = State.Operator;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(token.kind), token.kind, "Unknown TokenKind");
            }
        }
        void EvalGE(Order pre)
        {
            // Evaluate all functions with higher or equal precedence, used by left associative operators
            while (functions.Peek()?.order >= pre)
            {
                EvalApply();
            }
        }
        void EvalGT(Order pre)
        {
            // Evaluate all functions with strictly higher precedence, used by right associative operators and functions
            while (functions.Peek()?.order > pre)
            {
                EvalApply();
            }
        }
        void EvalApply()
        {
            // Pop the last function from the fn stack
            var head = functions.Pop();
            if (!head.HasValue)
                throw new InvalidOperationException("Attempting to apply a function when there are no more functions to apply, call a programmer!");
            // Gets the arguments for the function call
            var args = values.Top(head.Value.nargs);
            if (!args.HasValue)
                throw new InvalidOperationException("The function has more arguments than there are values on the value stack, call a programmer!");
            // Invoke the function with the arguments and push the result on the value stack
            double result = head.Value.function(environment, args.Value);
            values.Push(result);
        }
        #endregion
    }
}
