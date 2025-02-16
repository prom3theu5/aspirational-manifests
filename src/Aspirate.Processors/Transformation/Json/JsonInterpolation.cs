
using System.Text;

namespace Aspirate.Processors.Transformation.Json;

public static class JsonInterpolation
{
    public class JsonInterpolationTokenComparer : IEqualityComparer<JsonInterpolationToken>
    {
        public bool Equals(JsonInterpolationToken x, JsonInterpolationToken y) =>
            x.Lexeme == y.Lexeme && x.TokenType == y.TokenType;

        public int GetHashCode([DisallowNull] JsonInterpolationToken obj)
        {
            var hc = new HashCode();
            hc.Add(obj.Lexeme);
            hc.Add(obj.TokenType);

            return hc.ToHashCode();
        }
    }

    public enum JsonInterpolationTokenType
    {
        Text,
        Placeholder,
    }

    public readonly struct JsonInterpolationToken(
        JsonInterpolationTokenType tokenType,
        string lexeme)
    {
        // Suppressing this naming style convention as these are public fields.
#pragma warning disable IDE1006 // Naming Styles
        public readonly JsonInterpolationTokenType TokenType = tokenType;

        public readonly string Lexeme = lexeme;
#pragma warning restore IDE1006 // Naming Styles

        public override string ToString() => $"{TokenType}: '{Lexeme}'";

        public bool IsText() => TokenType == JsonInterpolationTokenType.Text;

        public bool IsPlaceholder() => TokenType == JsonInterpolationTokenType.Placeholder;
    }

    private enum TokenizerState
    {
        InText,
        InPlaceholderStart,
        InPlaceholder,
    }

    public static List<JsonInterpolationToken> Tokenize(string input) =>
        input is null ? [] : Tokenize(input.AsSpan());

    public static List<JsonInterpolationToken> Tokenize(ReadOnlySpan<char> input)
    {
        var state = TokenizerState.InText;
        var currentTokenIndex = 0;

        // Estimate the number of tokens.
        var tokens = new List<JsonInterpolationToken>(input.Length / 2);

        void TryAddToken(
            ref ReadOnlySpan<char> input,
            JsonInterpolationTokenType tokenType,
            int length,
            int nextTokenIndex)
        {
            if (length != 0 || tokenType != JsonInterpolationTokenType.Text)
            {
                tokens.Add(
                    new JsonInterpolationToken(
                        tokenType,
                        input.Slice(currentTokenIndex, length).ToString()));
            }

            currentTokenIndex = nextTokenIndex;
        }

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            switch (state)
            {
                case TokenizerState.InText:

                    if (currentChar == '{')
                    {
                        // We are in a potential placeholder token start.
                        state = TokenizerState.InPlaceholderStart;
                    }

                    break;

                case TokenizerState.InPlaceholderStart:

                    if (currentChar == '{')
                    {
                        // This curly brace is escaped, we're still in text.
                        state = TokenizerState.InText;
                    }
                    else
                    {
                        // We are in a placeholder token, slice the previous text.
                        TryAddToken(
                            ref input,
                            JsonInterpolationTokenType.Text,
                            i - currentTokenIndex - 1,
                            i);

                        // Advance to toke in placeholder token state.
                        state = TokenizerState.InPlaceholder;
                    }

                    break;

                case TokenizerState.InPlaceholder:

                    // We are going for parity with the regular expression used in the
                    // previous implementation: ([\w\.-]+)
                    if (currentChar is
                        (>= 'a' and <= 'z') or
                        (>= 'A' and <= 'Z') or
                        (>= '0' and <= '9') or
                        '.' or
                        '-')
                    {
                        // We are still in the placeholder token.
                        continue;
                    }
                    else if (currentChar == '}')
                    {
                        // Our placeholder token is complete.
                        TryAddToken(
                            ref input,
                            JsonInterpolationTokenType.Placeholder,
                            i - currentTokenIndex,
                            i + 1);

                        // The next token is probably text.
                        state = TokenizerState.InText;
                    }
                    else
                    {
                        // Our placeholder token has unsupported characters. As per
                        // the original implementation, we are going to treat the
                        // current lexme as text.
                        state = TokenizerState.InText;
                    }

                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        if (currentTokenIndex != input.Length - 1)
        {
            // There is a dangling token. To ensure parity with the original
            // implementation, we're going to treat it as text.

            if (state == TokenizerState.InPlaceholder)
            {
                // We were actually in a placeholder token, so we need to move
                // back a single character to ensure we catch the opening brace.
                currentTokenIndex--;
            }

            TryAddToken(
                ref input,
                JsonInterpolationTokenType.Text,
                input.Length - currentTokenIndex,
                0);
        }

        return tokens;
    }

    private enum UnescapeState
    {
        InText,
        InOpenBrace,
        InCloseBrace,
    }

    public static string Unescape(string value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var state = UnescapeState.InText;
        var unescaped = new StringBuilder();
        var span = value.AsSpan();
        var currentTokenIndex = 0;

        void Append(ref ReadOnlySpan<char> span, int length, int nextTokenIndex)
        {
            if (length > 0)
            {
                unescaped.Append(span.Slice(currentTokenIndex, length));
            }

            currentTokenIndex = nextTokenIndex;
        }

        for (var i = 0; i < span.Length; i++)
        {
            var currentChar = span[i];

            void HandleEscapedChar(ref ReadOnlySpan<char> span, char c)
            {
                if (currentChar == c)
                {
                    Append(ref span, i - currentTokenIndex, i + 1);
                }

                state = UnescapeState.InText;
            }

            switch (state)
            {
                case UnescapeState.InText:
                    switch (currentChar)
                    {
                        case '{':
                            state = UnescapeState.InOpenBrace;
                            break;

                        case '}':
                            state = UnescapeState.InCloseBrace;
                            break;
                    }
                    break;

                case UnescapeState.InOpenBrace:
                    HandleEscapedChar(ref span, '{');
                    break;

                case UnescapeState.InCloseBrace:
                    HandleEscapedChar(ref span, '}');
                    break;
            }
        }

        Append(ref span, span.Length - currentTokenIndex, 0);

        return unescaped.ToString();
    }
}
