using System.Text;
using Aspirate.Processors.Transformation.Json;
using static Aspirate.Processors.Transformation.Json.JsonInterpolation;

namespace Aspirate.Tests.ProcessorTests;

public class JsonInterpolationTests
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void TokenizeValue_ReturnsTokenList(string value, JsonInterpolationToken[] expectedTokens)
    {
        // Arrange
        var comparer = new JsonInterpolationTokenComparer();

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
                new(JsonInterpolationTokenType.Text, "foo"),
            ]),
        CreateTestData(
            "{x.y}",
            [
                new(JsonInterpolationTokenType.Placeholder, "x.y"),
            ]),
        CreateTestData(
            "",
            []),
        CreateTestData(
            "foo {x.y} bar",
            [
                new(JsonInterpolationTokenType.Text, "foo "),
                new(JsonInterpolationTokenType.Placeholder, "x.y"),
                new(JsonInterpolationTokenType.Text, " bar"),
            ]),
        CreateTestData(
            "foo {x.y}bar",
            [
                new(JsonInterpolationTokenType.Text, "foo "),
                new(JsonInterpolationTokenType.Placeholder, "x.y"),
                new(JsonInterpolationTokenType.Text, "bar"),
            ]),
        CreateTestData(
            "foo{x.y} bar",
            [
                new(JsonInterpolationTokenType.Text, "foo"),
                new(JsonInterpolationTokenType.Placeholder, "x.y"),
                new(JsonInterpolationTokenType.Text, " bar"),
            ]),
        CreateTestData(
            "foo{x.y}bar",
            [
                new(JsonInterpolationTokenType.Text, "foo"),
                new(JsonInterpolationTokenType.Placeholder, "x.y"),
                new(JsonInterpolationTokenType.Text, "bar"),
            ]),
        CreateTestData(
            "5230",
            [
                new(JsonInterpolationTokenType.Text, "5230"),
            ]),
        CreateTestData(
            "{5230}",
            [
                new(JsonInterpolationTokenType.Placeholder, "5230"),
            ]),
        CreateTestData(
            "{{5230}",
            [
                new(JsonInterpolationTokenType.Text, "{{5230}"),
            ]),
        CreateTestData(
            "{{5230",
            [
                new(JsonInterpolationTokenType.Text, "{{5230"),
            ]),
        CreateTestData(
            "{5230",
            [
                new(JsonInterpolationTokenType.Text, "{5230"),
            ]),
        CreateTestData(
            "5230}",
            [
                new(JsonInterpolationTokenType.Text, "5230}"),
            ]),
        CreateTestData(
            "5230}}",
            [
                new(JsonInterpolationTokenType.Text, "5230}}"),
            ]),
        CreateTestData(
            "5230}}}",
            [
                new(JsonInterpolationTokenType.Text, "5230}}}"),
            ]),
        CreateTestData(
            "5230}tail",
            [
                new(JsonInterpolationTokenType.Text, "5230}tail"),
            ]),
        CreateTestData(
            "5230}}tail",
            [
                new(JsonInterpolationTokenType.Text, "5230}}tail"),
            ]),
        CreateTestData(
            "5230}}}tail",
            [
                new(JsonInterpolationTokenType.Text, "5230}}}tail"),
            ]),
    ];

    private static object[] CreateTestData(string value, JsonInterpolationToken[] expectedTokens) => [value, expectedTokens];

    private static List<JsonInterpolationToken> JoinTextTokens(List<JsonInterpolationToken> tokens)
    {
        var sb = new StringBuilder();
        var joinedTokens = new List<JsonInterpolationToken>();

        void TryAddJoinedTextToken()
        {
            if (sb.Length == 0)
            {
                return;
            }

            joinedTokens.Add(new JsonInterpolationToken(JsonInterpolationTokenType.Text, sb.ToString()));
            sb.Clear();
        }

        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case JsonInterpolationTokenType.Text:
                    sb.Append(token.Lexeme);
                    break;

                case JsonInterpolationTokenType.Placeholder:
                    TryAddJoinedTextToken();
                    joinedTokens.Add(token);
                    break;
            }
        }

        TryAddJoinedTextToken();

        return joinedTokens;
    }

    [Theory]
    [InlineData("foo", "foo")]
    [InlineData("{{foo}}", "{foo}")]
    [InlineData("{foo}", "{foo}")]
    [InlineData("{{foo", "{foo")]
    [InlineData("{foo", "{foo")]
    [InlineData("foo}}", "foo}")]
    [InlineData("foo}", "foo}")]
    [InlineData("foo {{bar}}", "foo {bar}")]
    [InlineData("foo {bar}", "foo {bar}")]
    [InlineData("foo {{bar}} test", "foo {bar} test")]
    [InlineData("foo {bar} test", "foo {bar} test")]
    [InlineData("foo {{bar test", "foo {bar test")]
    [InlineData("foo bar}} test", "foo bar} test")]
    [InlineData("foo {bar test", "foo {bar test")]
    [InlineData("foo bar} test", "foo bar} test")]
    [InlineData("{{", "{")]
    [InlineData("}}", "}")]
    [InlineData("{", "{")]
    [InlineData("}", "}")]
    [InlineData("{{}}", "{}")]
    [InlineData("{}", "{}")]
    [InlineData("", "")]
    public void UnescapeValue_ReturnUnescapedString(string escapedString, string expectedUnescapedString)
    {
        // Arrange

        // Act
        var actualUnescapedString = Unescape(escapedString);

        // Assert
        Assert.Equal(expectedUnescapedString, actualUnescapedString);
    }
}
