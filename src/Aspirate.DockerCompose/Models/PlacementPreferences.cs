namespace Aspirate.DockerCompose.Models;

[Serializable]
public class PlacementPreferences : ObjectBase
{
    public string Spread
    {
        get => GetProperty<string>("spread")!;
        set => SetProperty("spread", value);
    }
}
