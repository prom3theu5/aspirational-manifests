using System.Text;
using static Aspirate.Processors.Transformation.Json.PlaceholderTokenizer;

namespace Aspirate.Tests.ProcessorTests;

public class PlaceholderTokenizerTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void TokenizeValue_ReturnsTokenList(string value, PlaceholderToken[] expectedTokens)
    {
        // Arrange
        var comparer = new PlaceholderTokenComparer();

        // Act
        var actualTokens = JoinTextTokens(Tokenize(value));

        // Assert
        Assert.Equal(expectedTokens, actualTokens, comparer);
    }

    public static IEnumerable<object[]> TestData =>
    [
        CreateTestData(
            "foo",
            [
                new(PlaceholderTokenType.Text, "foo"),
            ]),
        CreateTestData(
            "{x.y}",
            [
                new(PlaceholderTokenType.Placeholder, "x.y"),
            ]),
        CreateTestData(
            "",
            []),
        CreateTestData(
            "foo {x.y} bar",
            [
                new(PlaceholderTokenType.Text, "foo "),
                new(PlaceholderTokenType.Placeholder, "x.y"),
                new(PlaceholderTokenType.Text, " bar"),
            ]),
        CreateTestData(
            "foo {x.y}bar",
            [
                new(PlaceholderTokenType.Text, "foo "),
                new(PlaceholderTokenType.Placeholder, "x.y"),
                new(PlaceholderTokenType.Text, "bar"),
            ]),
        CreateTestData(
            "foo{x.y} bar",
            [
                new(PlaceholderTokenType.Text, "foo"),
                new(PlaceholderTokenType.Placeholder, "x.y"),
                new(PlaceholderTokenType.Text, " bar"),
            ]),
        CreateTestData(
            "foo{x.y}bar",
            [
                new(PlaceholderTokenType.Text, "foo"),
                new(PlaceholderTokenType.Placeholder, "x.y"),
                new(PlaceholderTokenType.Text, "bar"),
            ]),
        CreateTestData(
            "5230",
            [
                new(PlaceholderTokenType.Text, "5230"),
            ]),
        CreateTestData(
            "{5230}",
            [
                new(PlaceholderTokenType.Placeholder, "5230"),
            ]),
        CreateTestData(
            "{{5230}",
            [
                new(PlaceholderTokenType.Text, "{5230}"),
            ]),
        CreateTestData(
            "{{5230",
            [
                new(PlaceholderTokenType.Text, "{5230"),
            ]),
        CreateTestData(
            "{5230",
            [
                new(PlaceholderTokenType.Text, "{5230"),
            ]),
        CreateTestData(
            "5230}}",
            [
                new(PlaceholderTokenType.Text, "5230}"),
            ]),
        CreateTestData(
            "5230}}}",
            [
                new(PlaceholderTokenType.Text, "5230}}"),
            ]),
        CreateTestData(
            "5230}}tail",
            [
                new(PlaceholderTokenType.Text, "5230}tail"),
            ]),
        CreateTestData(
            "5230}tail",
            [
                new(PlaceholderTokenType.Text, "5230}tail"),
            ]),
    ];

    private static object[] CreateTestData(string value, PlaceholderToken[] expectedTokens) => [value, expectedTokens];

    private static List<PlaceholderToken> JoinTextTokens(List<PlaceholderToken> tokens)
    {
        var sb = new StringBuilder();
        var joinedTokens = new List<PlaceholderToken>();

        void TryAddJoinedTextToken()
        {
            if (sb.Length == 0)
            {
                return;
            }

            joinedTokens.Add(new PlaceholderToken(PlaceholderTokenType.Text, sb.ToString()));
            sb.Clear();
        }

        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case PlaceholderTokenType.Text:
                    sb.Append(token.Lexeme);
                    break;

                case PlaceholderTokenType.Placeholder:
                    TryAddJoinedTextToken();
                    joinedTokens.Add(token);
                    break;
            }
        }

        TryAddJoinedTextToken();

        return joinedTokens;
    }
}
