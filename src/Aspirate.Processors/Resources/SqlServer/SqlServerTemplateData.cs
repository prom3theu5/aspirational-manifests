namespace Aspirate.Processors.Resources.SqlServer;

public sealed class SqlServerTemplateData : KubernetesDeploymentTemplateData
{
    public string SaPassword { get; set; } = default!;

    public KubernetesDeploymentTemplateData SetSaPassword(string password)
    {
        SaPassword = password;
        return this;
    }
}
