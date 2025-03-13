namespace Aspirate.Shared.Extensions;

public static class StringExtensions
{
    public static string ToBase64(this string value)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string? FromBase64(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var base64EncodedBytes = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string AsJsonPath(this IEnumerable<string> pathParts)
    {
        var formattedParts = pathParts.Select(part => $"['{part}']");
        return "$" + string.Join("", formattedParts);
    }
}
