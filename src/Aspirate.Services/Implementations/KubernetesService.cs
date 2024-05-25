namespace Aspirate.Services.Implementations;

public class KubernetesService(IAnsiConsole logger, IKubeCtlService kubeCtlService, IServiceProvider serviceProvider)
    : IKubernetesService
{
    public List<object> ConvertResourcesToKubeObjects(List<KeyValuePair<string, Resource>> supportedResources, AspirateState state, bool encodeSecrets)
    {
        var kubernetesObjects = new List<object>();

        if (!string.IsNullOrEmpty(state.Namespace) && state.UseCustomNamespace == true)
        {
            kubernetesObjects.Add(new V1Namespace { Metadata = new V1ObjectMeta { Name = state.Namespace, }, });
        }

        foreach (var resource in supportedResources)
        {
            kubernetesObjects.AddRange(ProcessIndividualResourceManifests(resource, state, encodeSecrets));
        }

        if (state.IncludeDashboard == true)
        {
            kubernetesObjects.AddRange(CreateDashboardKubernetesObjects());
        }

        return kubernetesObjects;
    }

    public async Task<bool> DeleteNamespace(KubernetesRunOptions options)
    {
        try
        {
            await options.Client.CoreV1.DeleteNamespaceAsync(options.NamespaceName);
            await WaitForDeletion(() => options.Client.CoreV1.ReadNamespaceAsync(options.NamespaceName));

            logger.MarkupLine($"[green]Namespace '{options.NamespaceName}' has been deleted.[/]");

            return true;
        }
        catch (Exception e)
        {
            logger.MarkupLine($"[red]Failed to delete namespace '{options.NamespaceName}'.[/]");
            logger.MarkupLine($"[red]{e.Message}[/]");
            return false;
        }
    }

    public IKubernetes CreateClient(string clusterName)
    {
        if (string.IsNullOrEmpty(clusterName))
        {
            logger.MarkupLine("[red]Failed to set active kubernetes context.[/]");
            ActionCausesExitException.ExitNow();
        }

        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(currentContext: clusterName);
        return new Kubernetes(config);
    }

    public async Task InteractivelySelectKubernetesCluster(AspirateState state)
    {
        if (state.ActiveKubernetesContextIsSet)
        {
            return;
        }

        var shouldDeploy = logger.Confirm(
            "[bold]Would you like to deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file?[/]");

        if (!shouldDeploy)
        {
            logger.MarkupLine("[yellow]Skipping deployment of manifests to cluster.[/]");
            ActionCausesExitException.ExitNow();
        }

        state.KubeContext = await kubeCtlService.SelectKubernetesContextForDeployment();

        if (string.IsNullOrEmpty(state.KubeContext))
        {
            logger.MarkupLine("[red]Failed to set active kubernetes context.[/]");
            ActionCausesExitException.ExitNow();
        }
    }

    public async Task<bool> ClearNamespace(KubernetesRunOptions options)
    {
        try
        {
            await DeleteAllIngresses(options);
            await DeleteAllServices(options);
            await DeleteAllDeployments(options);
            await DeleteAllStatefulSets(options);
            await DeleteAllPersistentVolumes(options);
            await DeleteAllConfigMaps(options);
            await DeleteAllSecrets(options);

            logger.MarkupLine($"[green]Namespace '{options.NamespaceName}' has been cleared.[/]");

            return true;
        }
        catch (Exception e)
        {
            logger.MarkupLine($"[red]Failed to clear namespace '{options.NamespaceName}'.[/]");
            logger.MarkupLine($"[red]{e.Message}[/]");
            return false;
        }
    }

    public async Task<bool> ApplyObjectsToCluster(KubernetesRunOptions options)
    {
        options.Validate(logger);

        // Order the objects so that we have a valid order of operation...
        var orderedObjects = options.KubernetesObjects.OrderBy(obj =>
        {
            return obj switch
            {
                V1Namespace _ => 0,
                V1ConfigMap _ => 1,
                V1Secret _ => 2,
                V1Deployment _ => 3,
                V1StatefulSet _ => 4,
                V1Service => 5,
                _ => 6
            };
        });

        if (!await IsNamespaceEmpty(options))
        {
            logger.MarkupLine($"[yellow]Namespace '{options.NamespaceName}' is not empty. Skipping resource creation.[/]");
            return false;
        }

        foreach (var obj in orderedObjects)
        {
            switch (obj)
            {
                case V1Namespace @namespace:
                    await HandleNamespaceCreation(options, @namespace);
                    break;
                case V1ConfigMap configMap:
                    configMap.Metadata.NamespaceProperty = options.NamespaceName;
                    await options.Client.CoreV1.CreateNamespacedConfigMapAsync(configMap, options.NamespaceName);
                    break;
                case V1Secret secret:
                    secret.Metadata.NamespaceProperty = options.NamespaceName;
                    await options.Client.CoreV1.CreateNamespacedSecretAsync(secret, options.NamespaceName);
                    break;
                case V1Deployment deployment:
                    deployment.Metadata.NamespaceProperty = options.NamespaceName;
                    await options.Client.AppsV1.CreateNamespacedDeploymentAsync(deployment, options.NamespaceName);
                    break;
                case V1StatefulSet statefulSet:
                    statefulSet.Metadata.NamespaceProperty = options.NamespaceName;
                    await options.Client.AppsV1.CreateNamespacedStatefulSetAsync(statefulSet, options.NamespaceName);
                    break;
                case V1Service service:
                    service.Metadata.NamespaceProperty = options.NamespaceName;
                    if (service.Spec.Ports.Where(ExposableAsNodePort).Any())
                    {
                        service.Spec.Type = "NodePort";
                    }
                    await options.Client.CoreV1.CreateNamespacedServiceAsync(service, options.NamespaceName);
                    break;
            }
        }

        return true;
    }

    public async Task ListServiceAddresses(KubernetesRunOptions options)
    {
        logger.WriteRuler("[purple]Deployment completion: Outputting service details[/]");

        var services = await options.Client.CoreV1.ListNamespacedServiceAsync(options.NamespaceName);

        var table = new Table()
            .AddColumn("Service Name")
            .AddColumn("Service Type")
            .AddColumn("Cluster IP")
            .AddColumn("Port")
            .AddColumn("Node Port")
            .AddColumn("Address");

        foreach (var service in services.Items)
        {
            var serviceName = service.Metadata.Name;
            var serviceType = service.Spec.Type;
            var clusterIP = service.Spec.ClusterIP;
            var ports = service.Spec.Ports.Where(ExposableAsNodePort);

            foreach (var port in ports)
            {
                if (serviceType == "NodePort")
                {
                    var nodePort = port.NodePort;
                    var address = $"http://localhost:{nodePort}";

                    table.AddRow(serviceName, serviceType, clusterIP, port.Port.ToString(), nodePort.ToString(), address);
                }
            }
        }

        logger.Render(table);
    }

    public async Task<bool> IsNamespaceEmpty(KubernetesRunOptions options)
    {
        var ingresses = await options.Client.NetworkingV1.ListNamespacedIngressAsync(options.NamespaceName);
        var services = await options.Client.CoreV1.ListNamespacedServiceAsync(options.NamespaceName);
        var deployments = await options.Client.AppsV1.ListNamespacedDeploymentAsync(options.NamespaceName);
        var statefulSets = await options.Client.AppsV1.ListNamespacedStatefulSetAsync(options.NamespaceName);
        var configMaps = await options.Client.CoreV1.ListNamespacedConfigMapAsync(options.NamespaceName);
        var secrets = await options.Client.CoreV1.ListNamespacedSecretAsync(options.NamespaceName);

        return ingresses.Items.Count == 0 &&
               services.Items.Count == 0 &&
               deployments.Items.Count == 0 &&
               statefulSets.Items.Count == 0 &&
               configMaps.Items.All(x => x.Metadata.Name.Equals("kube-root-ca.crt", StringComparison.OrdinalIgnoreCase)) &&
               secrets.Items.Count == 0;
    }

    public List<object> CreateDashboardKubernetesObjects()
    {
        var labels = new Dictionary<string, string>
        {
            ["app"] = "aspire-dashboard",
        };

        var deployment = AspireDashboard.GetDeployment(labels);
        var service = AspireDashboard.GetService(labels);

        return
        [
            deployment,
            service
        ];
    }

    private async Task HandleNamespaceCreation(KubernetesRunOptions options, V1Namespace @namespace)
    {
        try
        {
            _ = await options.Client.CoreV1.ReadNamespaceAsync(@namespace.Metadata.Name);
            logger.MarkupLine($"[yellow]Namespace '{@namespace.Metadata.Name}' already exists. Skipping creation of new namespace.[/]");
        }
        catch (HttpOperationException e) when (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await options.Client.CoreV1.CreateNamespaceAsync(@namespace);
        }
        catch (Exception e)
        {
            logger.MarkupLine($"[red]Failed to create namespace '{@namespace.Metadata.Name}'.[/]");
            logger.MarkupLine($"[red]{e.Message}[/]");
            ActionCausesExitException.ExitNow();
        }
    }

    private List<object> ProcessIndividualResourceManifests(KeyValuePair<string, Resource> resource, AspirateState state, bool encodeSecrets)
    {
        var handler = serviceProvider.GetKeyedService<IResourceProcessor>(resource.Value.Type);

        if (handler is null)
        {
            logger.MarkupLine($"[yellow]Skipping resource '{resource.Key}' as its type is unsupported.[/]");
            return [];
        }

        return handler.CreateKubernetesObjects(new()
        {
            Resource = resource,
            ImagePullPolicy = state.ImagePullPolicy,
            DisableSecrets = state.DisableSecrets,
            WithPrivateRegistry = state.WithPrivateRegistry,
            WithDashboard = state.IncludeDashboard,
            EncodeSecrets = encodeSecrets,
        });
    }

    private async Task WaitForDeletion(Func<Task> readFunc)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(2));

        while (true)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            try
            {
                await readFunc();
                await Task.Delay(1000, cancellationTokenSource.Token);
            }
            catch (HttpOperationException e) when (e.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                break;
            }
            catch (OperationCanceledException)
            {
                logger.MarkupLine("[red]Timed out waiting for resource deletion.[/]");
                ActionCausesExitException.ExitNow();
            }
        }
    }

        private async Task DeleteAllSecrets(KubernetesRunOptions options)
    {
        var secrets = await options.Client.CoreV1.ListNamespacedSecretAsync(options.NamespaceName);
        foreach (var secret in secrets.Items)
        {
            logger.MarkupLine($"[yellow]Deleting secret '{secret.Metadata.Name}'[/]");
            await options.Client.CoreV1.DeleteNamespacedSecretAsync(secret.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.CoreV1.ReadNamespacedSecretAsync(secret.Metadata.Name, options.NamespaceName));
        }
    }

    private async Task DeleteAllConfigMaps(KubernetesRunOptions options)
    {
        var configMaps = await options.Client.CoreV1.ListNamespacedConfigMapAsync(options.NamespaceName);
        foreach (var configMap in configMaps.Items.Where(x=> !x.Metadata.Name.Equals("kube-root-ca.crt", StringComparison.OrdinalIgnoreCase)))
        {
            logger.MarkupLine($"[yellow]Deleting config map '{configMap.Metadata.Name}'[/]");
            await options.Client.CoreV1.DeleteNamespacedConfigMapAsync(configMap.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.CoreV1.ReadNamespacedConfigMapAsync(configMap.Metadata.Name, options.NamespaceName));
        }
    }

    private async Task DeleteAllStatefulSets(KubernetesRunOptions options)
    {
        var statefulSets = await options.Client.AppsV1.ListNamespacedStatefulSetAsync(options.NamespaceName);
        foreach (var statefulSet in statefulSets.Items)
        {
            logger.MarkupLine($"[yellow]Deleting stateful set '{statefulSet.Metadata.Name}'[/]");
            await options.Client.AppsV1.DeleteNamespacedStatefulSetAsync(statefulSet.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.AppsV1.ReadNamespacedStatefulSetAsync(statefulSet.Metadata.Name, options.NamespaceName));
        }
    }

    private async Task DeleteAllPersistentVolumes(KubernetesRunOptions options)
    {
        var persistentVolumes = await options.Client.CoreV1.ListPersistentVolumeAsync();
        foreach (var persistentVolume in persistentVolumes.Items)
        {
            logger.MarkupLine($"[yellow]Deleting persistent volume '{persistentVolume.Metadata.Name}'[/]");
            await options.Client.CoreV1.DeletePersistentVolumeAsync(persistentVolume.Metadata.Name);
            await WaitForDeletion(() =>
                options.Client.CoreV1.ReadPersistentVolumeAsync(persistentVolume.Metadata.Name));
        }
    }

    private async Task DeleteAllDeployments(KubernetesRunOptions options)
    {
        var deployments = await options.Client.AppsV1.ListNamespacedDeploymentAsync(options.NamespaceName);
        foreach (var deployment in deployments.Items)
        {
            logger.MarkupLine($"[yellow]Deleting deployment '{deployment.Metadata.Name}'[/]");
            await options.Client.AppsV1.DeleteNamespacedDeploymentAsync(deployment.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.AppsV1.ReadNamespacedDeploymentAsync(deployment.Metadata.Name, options.NamespaceName));
        }
    }

    private async Task DeleteAllServices(KubernetesRunOptions options)
    {
        var services = await options.Client.CoreV1.ListNamespacedServiceAsync(options.NamespaceName);
        foreach (var service in services.Items)
        {
            logger.MarkupLine($"[yellow]Deleting service '{service.Metadata.Name}'[/]");
            await options.Client.CoreV1.DeleteNamespacedServiceAsync(service.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.CoreV1.ReadNamespacedServiceAsync(service.Metadata.Name, options.NamespaceName));
        }
    }

    private async Task DeleteAllIngresses(KubernetesRunOptions options)
    {
        var ingresses = await options.Client.NetworkingV1.ListNamespacedIngressAsync(options.NamespaceName);
        foreach (var ingress in ingresses.Items)
        {
            logger.MarkupLine($"[yellow]Deleting ingress '{ingress.Metadata.Name}'[/]");
            await options.Client.NetworkingV1.DeleteNamespacedIngressAsync(ingress.Metadata.Name, options.NamespaceName);
            await WaitForDeletion(() =>
                options.Client.NetworkingV1.ReadNamespacedIngressAsync(ingress.Metadata.Name, options.NamespaceName));
        }
    }

    private Func<V1ServicePort, bool> ExposableAsNodePort =>
        servicePort =>
            servicePort.Name.Equals("http", StringComparison.OrdinalIgnoreCase) || servicePort.Port is 80 or 8080 or 18888;
}
