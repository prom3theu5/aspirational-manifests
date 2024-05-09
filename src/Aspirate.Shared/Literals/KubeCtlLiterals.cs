namespace Aspirate.Shared.Literals;

public static class KubeCtlLiterals
{
    public const string KubeCtlCommand = "kubectl";

    public const string KubeCtlApplyArgument = "apply";
    public const string KubeCtlDeleteArgument = "delete";
    public const string KubeCtlConfigArgument = "config";
    public const string KubeCtlViewArgument = "view";
    public const string KubeCtlOutputArgument = "-o";
    public const string KubeCtlOutputJsonArgument = "json";
    public const string KubeCtlUseContextArgument = "use-context";
    public const string KubeCtlKustomizeManifestsArgument = "-k";
    public const string KubeCtlRolloutArgument = "rollout";
    public const string KubeCtlNamespaceArgument = "-n";
    public const string KubeCtlNoWaitArgument = "--wait=false";
    public const string KubeCtlFileArgument = "-f";

    public const string KubeCtlContextsProperty = "contexts";
    public const string KubeCtlNameProperty = "name";
    public const string KubeCtlDeploymentProperty = "deployment";
    public const string KubeCtlRestartProperty = "restart";
    public const string KubeCtlDefaultNamespace = "default";
}
