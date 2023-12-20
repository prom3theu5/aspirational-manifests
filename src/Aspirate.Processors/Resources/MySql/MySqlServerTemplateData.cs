namespace Aspirate.Processors.Resources.MySql;

public sealed class MySqlServerTemplateData : KubernetesDeploymentTemplateData
{
    public string RootPassword { get; set; } = default!;

    public KubernetesDeploymentTemplateData SetRootPassword(string password)
    {
        RootPassword = password;
        return this;
    }
}
