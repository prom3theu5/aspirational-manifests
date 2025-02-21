namespace Aspirate.Secrets;

public class MaskedValue(
    string? value,
    char maskChar = '*',
    int unmaskedHeadLength = 2,
    int unmaskedTailLength = 2)
{
    public override string ToString()
    {
        if (value == null)
        {
            return string.Empty;
        }

        // We're going for best effort here--if secret is exceedingly small,
        // we are not going to show any of it unmasked.
        var unmaskedTotalLength = unmaskedHeadLength + unmaskedTailLength;

        if (value.Length < unmaskedTotalLength * 3)
        {
            return new string(maskChar, value.Length);
        }

        var unmaskedHead = value[..unmaskedHeadLength];
        var unmaskedTail = value[^unmaskedTailLength..];

        var masked = new StringBuilder(value.Length);
        masked.Append(unmaskedHead);
        masked.Append(maskChar, value.Length - unmaskedTotalLength);
        masked.Append(unmaskedTail);

        return masked.ToString();
    }
}
