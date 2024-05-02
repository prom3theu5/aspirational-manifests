namespace Aspirate.Shared.Enums;

public class ImagePullPolicy : SmartEnum<ImagePullPolicy, string>
{
    private ImagePullPolicy(string name, string value) : base(name, value)
    {
    }

    public static ImagePullPolicy IfNotPresent = new(nameof(IfNotPresent), "IfNotPresent");
    public static ImagePullPolicy Always = new(nameof(Always), "Always");
    public static ImagePullPolicy Never = new(nameof(Never), "Never");
}
