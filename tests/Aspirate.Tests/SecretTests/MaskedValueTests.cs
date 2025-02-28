using System.CommandLine;
using System.CommandLine.Help;
using System.Text;
using Aspirate.Cli;

namespace Aspirate.Tests.SecretTests;

public class MaskedValueTests
{
    [Theory]
    [InlineData("password", "********")]
    [InlineData("LongPasswordTest", "Lo************st")]
    [InlineData("5230", "****")]
    [InlineData("523", "***")]
    [InlineData("52", "**")]
    [InlineData("5", "*")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void MaskedValue_ShouldMaskString(string? value, string expectedMaskedString)
    {
        // Arrange
        var maskedValue = new MaskedValue(value);

        // Act
        var actualMaskedString = maskedValue.ToString();

        // Assert
        Assert.Equal(expectedMaskedString, actualMaskedString);
    }

    [Theory]
    [InlineData("password", "********")]
    [InlineData("LongPasswordTest", "Lo************st")]
    [InlineData("5230", "****")]
    public void HelpBuilder_ShouldEmitMaskedString(string? value, string expectedMaskedString)
    {
        // Arrange
        var expectedDefaultArgument = FormatDefaultArgument(expectedMaskedString);
        var unexpectedSecretDefaultArgument = FormatDefaultArgument(value);
        Environment.SetEnvironmentVariable("ASPIRATE_SECRET_PASSWORD", value);
        var cli = new AspirateCli();
        var parseResult = cli.Parse(["generate", "--secret-password", value]);
        var helpBuilder = new HelpBuilder(LocalizationResources.Instance);
        var textOut = new StringBuilder();
        using var writer = new StringWriter(textOut);
        var helpContext = new HelpContext(helpBuilder, parseResult.CommandResult.Command, writer, parseResult);
        AspirateCli.UseDefaultMasking(helpContext);

        // Act
        helpBuilder.Write(helpContext);
        writer.Flush();
        var actualHelpOutput = textOut.ToString();

        // Assert
        Assert.Contains(expectedDefaultArgument, actualHelpOutput);
        Assert.DoesNotContain(unexpectedSecretDefaultArgument, actualHelpOutput);
    }

    private static string FormatDefaultArgument(string defaultArgument) => $"[default: {defaultArgument}]";
}
