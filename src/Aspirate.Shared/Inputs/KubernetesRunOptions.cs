namespace Aspirate.Shared.Inputs;

public class KubernetesRunOptions
{
    public required IKubernetes Client { get; set; }

    public required AspirateState CurrentState { get; set; }
    public required List<object> KubernetesObjects { get; set; }

    public string NamespaceName { get; set; } = "default";

    public void Validate(IAnsiConsole logger)
    {
        if (string.IsNullOrEmpty(NamespaceName))
        {
            logger.MarkupLine("[red]Namespace name is required.[/]");
            ActionCausesExitException.ExitNow();
        }

        if (KubernetesObjects.Count == 0)
        {
            logger.MarkupLine("[red]No Kubernetes objects to deploy.[/]");
            ActionCausesExitException.ExitNow();
        }
    }
}
