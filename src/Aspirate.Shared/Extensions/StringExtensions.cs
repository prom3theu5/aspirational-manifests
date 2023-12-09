namespace Aspirate.Shared.Extensions;

public static class StringExtensions
{
    public static string ToBase64(this string value)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(plainTextBytes);
    }
}
