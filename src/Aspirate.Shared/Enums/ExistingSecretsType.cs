namespace Aspirate.Shared.Enums;

public class ExistingSecretsType : SmartEnum<ExistingSecretsType, string>
{
    private ExistingSecretsType(string name, string value) : base(name, value)
    {
    }

    public static ExistingSecretsType Existing = new(nameof(Existing), "Use Existing");
    public static ExistingSecretsType Augment = new(nameof(Augment), "Augment by adding / replacing values");
    public static ExistingSecretsType Overwrite = new(nameof(Overwrite), "Overwrite / Create new Password");
}
