using System.Collections.Generic;
using System.Globalization;

namespace Pupil
{
    /// <summary>
    /// The kinds of tokens the lexer may produce.
    /// </summary>
    public enum TokenKind
    {
        /// <summary>
        /// The string is syntactically invalid.
        /// 
        /// <see cref="Token.value"/> is a <see cref="int"/>: the index in the string where parsing went wrong.
        /// </summary>
        Unknown,
        /// <summary>
        /// A real number literal.
        /// 
        /// <see cref="Token.value"/> is a <see cref="double"/>: the parsed value.
        /// </summary>
        Literal,
        /// <summary>
        /// An operator symbol.
        /// 
        /// <see cref="Token.value"/> is a <see cref="Operator"/>: the operator value.
        /// </summary>
        Operator,
        /// <summary>
        /// A named variable.
        /// 
        /// <see cref="Token.value"/> is a <see cref="string"/>: the identifier of the variable referenced.
        /// </summary>
        Variable,
        /// <summary>
        /// A named function call. Functions are identifiers followed by an opening parenthesis.
        /// 
        /// <see cref="Token.value"/> is a <see cref="string"/>: the identifier of the function referenced.
        /// </summary>
        Function,
        /// <summary>
        /// A comma used to separate arguments to a function call.
        /// 
        /// <see cref="Token.value"/> is null.
        /// </summary>
        Comma,
        /// <summary>
        /// A closing parenthesis.
        /// </summary>
        EndFunction,
    }

    /// <summary>
    /// Tokens are produced by the lexer's tokenizer as a first step to parse the string.
    /// </summary>
    public struct Token
    {
        /// <summary>
        /// The kind of token that was found.
        /// </summary>
        public TokenKind kind;
        /// <summary>
        /// Depending on the <see cref="Token.kind"/> contains some context information. See <see cref="TokenKind"/> for expected types and values.
        /// </summary>
        public object value;
    }

    public class Lexer
    {
        /// <summary>
        /// Tokenize the input string for parsing.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<Token> Tokenize(string input)
        {
            int index = 0;
            while (index < input.Length)
            {
                // Start by skipping over the whitespace
                while (index < input.Length && char.IsWhiteSpace(input[index]))
                    ++index;
                if (index >= input.Length)
                    yield break;

                // Lex operators
                switch (input[index])
                {
                    case '+':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Add };
                        continue;
                    case '-':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Sub };
                        continue;
                    case '*':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Mul };
                        continue;
                    case '/':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Div };
                        continue;
                    case '%':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Rem };
                        continue;
                    case '^':
                        index += 1;
                        yield return new Token { kind = TokenKind.Operator, value = Operator.Exp };
                        continue;
                    case ',':
                        index += 1;
                        yield return new Token { kind = TokenKind.Comma, value = null };
                        continue;
                    case ')':
                        index += 1;
                        yield return new Token { kind = TokenKind.EndFunction, value = null };
                        continue;
                }

                // Lex literals
                int jndex = index;
                while (jndex < input.Length && (input[jndex] >= '0' && input[jndex] <= '9' || input[jndex] == '.'))
                    ++jndex;
                double literal;
                if (jndex > index && double.TryParse(input.Substring(index, jndex - index), NumberStyles.Float, CultureInfo.InvariantCulture, out literal))
                {
                    index = jndex;
                    yield return new Token { kind = TokenKind.Literal, value = literal };
                    continue;
                }

                // Lex identifiers
                int andex = index;
                while (andex < input.Length && char.IsLetterOrDigit(input[andex]))
                    ++andex;
                if (andex < input.Length && input[andex] == '(')
                {
                    string name = input.Substring(index, andex - index);
                    index = andex + 1;
                    yield return new Token { kind = TokenKind.Function, value = name };
                    continue;
                }
                else if (andex > index)
                {
                    string name = input.Substring(index, andex - index);
                    index = andex;
                    yield return new Token { kind = TokenKind.Variable, value = name };
                    continue;
                }

                // Fall through the 'Unknown' category, let upstream handle it and abort the generator
                int undex = index;
                index = input.Length;
                yield return new Token { kind = TokenKind.Unknown, value = undex };
            }
        }
    }
}
