namespace Aspirate.Processors.Transformation.Json;

public static class PlaceholderTokenizer
{
    public enum PlaceholderTokenType
    {
        Text,
        PlaceHolder,
    }

    public readonly struct PlaceholderToken(
        PlaceholderTokenType tokenType,
        string lexeme)
    {
        // Suppressing this naming style convention as these are public fields.
#pragma warning disable IDE1006 // Naming Styles
        public readonly PlaceholderTokenType TokenType = tokenType;

        public readonly string Lexeme = lexeme;
#pragma warning restore IDE1006 // Naming Styles

        public override string ToString() => $"{TokenType}: '{Lexeme}'";
    }

    private enum PlaceholderTokenizerState
    {
        InText,
        InPlaceholderStart,
        InPlaceholder,
        InEscapedPlaceholderEnd,
    }

    public static List<PlaceholderToken> Tokenize(string input) => Tokenize(input.AsSpan());

    public static List<PlaceholderToken> Tokenize(ReadOnlySpan<char> input)
    {
        var state = PlaceholderTokenizerState.InText;
        var currentTokenIndex = 0;

        // Estimate the number of tokens.
        var tokens = new List<PlaceholderToken>(input.Length / 2);

        void TryAddToken(
            ref ReadOnlySpan<char> input,
            PlaceholderTokenType tokenType,
            int length,
            int nextTokenIndex)
        {
            if (length == 0 && tokenType == PlaceholderTokenType.Text)
            {
                return;
            }

            tokens.Add(
                new PlaceholderToken(
                    tokenType,
                    input.Slice(currentTokenIndex, length).ToString()));

            currentTokenIndex = nextTokenIndex;
        }

        for (var i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            switch (state)
            {
                case PlaceholderTokenizerState.InText:

                    if (currentChar == '{')
                    {
                        // We are in a potential placeholder token start.
                        state = PlaceholderTokenizerState.InPlaceholderStart;
                    }
                    else if (currentChar == '}')
                    {
                        // We are in what can only be an escaped placeholder token end.
                        state = PlaceholderTokenizerState.InEscapedPlaceholderEnd;
                    }

                    break;

                case PlaceholderTokenizerState.InPlaceholderStart:

                    if (currentChar == '{')
                    {
                        // This curly brace is escaped, we're still in text.
                        state = PlaceholderTokenizerState.InText;
                    }
                    else
                    {
                        // We are in a placeholder token, slice the previous text.
                        TryAddToken(
                            ref input,
                            PlaceholderTokenType.Text,
                            i - currentTokenIndex - 1,
                            i);

                        // Advance to toke in placeholder token state.
                        state = PlaceholderTokenizerState.InPlaceholder;
                    }

                    break;

                case PlaceholderTokenizerState.InPlaceholder:

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
                            PlaceholderTokenType.PlaceHolder,
                            i - currentTokenIndex,
                            i + 1);

                        // The next token is probably text.
                        state = PlaceholderTokenizerState.InText;
                    }
                    else
                    {
                        // Our placeholder token has unsupported characters. As per
                        // the original implementation, we are going to treat the
                        // current lexme as text.
                        state = PlaceholderTokenizerState.InText;
                    }

                    break;

                case PlaceholderTokenizerState.InEscapedPlaceholderEnd:
                    // The name of this state is something of a misnomer. While we
                    // do in fact handle escaped close braces here, we will also
                    // accept unescaped, unbalanced close braces. This is done to
                    // ensure parity with the previous implementation.
                    if (currentChar == '}')
                    {
                        TryAddToken(
                            ref input,
                            PlaceholderTokenType.Text,
                            i - currentTokenIndex - 1,
                            i);
                    }

                    state = PlaceholderTokenizerState.InText;

                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        if (currentTokenIndex != input.Length - 1)
        {
            // There is a dangling token. To ensure parity with the original
            // implementation, we're going to treat it as text.
            TryAddToken(
                ref input,
                PlaceholderTokenType.Text,
                input.Length - currentTokenIndex,
                0);
        }

        return tokens;
    }
}
