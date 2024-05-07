namespace Aspirate.Shared.Models.AspireManifests;

public static class AspireDashboard
{
    public static V1Deployment GetDeployment(Dictionary<string, string> labels) =>
        new()
        {
            ApiVersion = "apps/v1",
            Kind = "Deployment",
            Metadata = new V1ObjectMeta { Name = "aspire-dashboard", Labels = labels, },
            Spec = new V1DeploymentSpec
            {
                Replicas = 1,
                Selector =
                    new V1LabelSelector { MatchLabels = labels },
                Template = new V1PodTemplateSpec
                {
                    Metadata =
                        new V1ObjectMeta { Labels = labels, },
                    Spec = new V1PodSpec
                    {
                        TerminationGracePeriodSeconds = 30,
                        Containers = new List<V1Container>
                        {
                            new()
                            {
                                Name = "aspire-dashboard",
                                Image = AspireLiterals.DashboardImage,
                                Resources =
                                    new V1ResourceRequirements
                                    {
                                        Requests = new Dictionary<string, ResourceQuantity>
                                        {
                                            ["cpu"] = new("500m"), ["memory"] = new("512Mi"),
                                        },
                                        Limits =
                                            new Dictionary<string, ResourceQuantity> { ["memory"] = new("512Mi"), }
                                    },
                                Ports =
                                    new List<V1ContainerPort>
                                    {
                                        new() { Name = "dashboard-ui", ContainerPort = 18888 },
                                        new() { Name = "otlp", ContainerPort = 18889 }
                                    },
                                Env = new List<V1EnvVar>
                                {
                                    new() { Name = "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS", Value = "true" }
                                },
                                LivenessProbe =
                                    new V1Probe
                                    {
                                        InitialDelaySeconds = 30,
                                        PeriodSeconds = 10,
                                        HttpGet = new V1HTTPGetAction { Path = "/", Port = 18888 }
                                    },
                                ReadinessProbe = new V1Probe
                                {
                                    InitialDelaySeconds = 30,
                                    PeriodSeconds = 10,
                                    HttpGet = new V1HTTPGetAction { Path = "/", Port = 18888 }
                                }
                            }
                        }
                    }
                }
            }
        };

    public static V1Service GetService(Dictionary<string, string> labels) =>
        new()
        {
            ApiVersion = "v1",
            Kind = "Service",
            Metadata = new V1ObjectMeta { Name = "aspire-dashboard" },
            Spec = new V1ServiceSpec
            {
                Selector = labels,
                Ports = new List<V1ServicePort>
                {
                    new() { Name = "dashboard-ui", Protocol = "TCP", Port = 18888, TargetPort = 18888 },
                    new() { Name = "otlp", Protocol = "TCP", Port = 18889, TargetPort = 18889 }
                },
                Type = "ClusterIP"
            }
        };
}
