# Running Solution Directly

The run command is used to run the solution directly from the AppHost directory against a cluster within your KUBECONFIG.

The command will first create the manifest file, however, this can be overridden if you pass in the path
to an existing manifest file using the `--aspire-manifest` or `-m` flag and supplying the path.

```bash
  aspirate run
```

```bash
  aspirate run -m ./manifest.json
```

To clean-up after using the run command, you can use the `Stop` command.
    
    ```bash
    aspirate stop
    ```

This deletes anything added to the Namespace within the state file, and removes the namespace.

## Cli Options (Optional)

| Option                        | Alias | Environmental Variable Counterpart     | Description                                                                                                                                                                    |
|-------------------------------|-------|----------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| --project-path                | -p    | `ASPIRATE_PROJECT_PATH`                | The path to the aspire project.                                                                                                                                                |
| --aspire-manifest             | -m    | `ASPIRATE_ASPIRE_MANIFEST_PATH`        | The aspire manifest file to use                                                                                                                                                |
| --skip-build                  |       | `ASPIRATE_SKIP_BUILD`                  | Skips build and Push of containers.                                                                                                                                            |
| --namespace                   |       | `ASPIRATE_NAMESPACE`                   | Generates a Kubernetes Namespace resource, and applies the namespace to all generated resources. Will be used at deployment time.                                              |
| --container-image-tag         | -ct   | `ASPIRATE_CONTAINER_IMAGE_TAG`         | The Container Image Tag to use as the fall-back value for all containers.                                                                                                      |
| --container-registry          | -cr   | `ASPIRATE_CONTAINER_REGISTRY`          | The Container Registry to use as the fall-back value for all containers.                                                                                                       |
| --container-repository-prefix |       | `ASPIRATE_CONTAINER_REPOSITORY_PREFIX` | The Container Repository Prefix to use as the fall-back value for all containers.                                                                                              |
| --container-builder           |       | `ASPIRATE_CONTAINER_BUILDER`           | The Container Builder: can be `docker` or `podman`. The default is `docker`.                                                                                                   |
| --image-pull-policy           |       | `ASPIRATE_IMAGE_PULL_POLICY`           | The image pull policy to use for all containers in generated manifests. Can be `Always`, `Never` or `IfNotPresent`. For your local docker desktop cluster - use `IfNotPresent` |
| --disable-secrets             |       | `ASPIRATE_DISABLE_SECRETS`             | Disables secrets management features.                                                                                                                                          |
| --runtime-identifier          |       | `ASPIRATE_RUNTIME_IDENTIFIER`          | Sets the runtime identifier for project builds. Defaults to `linux-x64`.                                                                                                       |
| --secret-password             |       | `ASPIRATE_SECRET_PASSWORD`             | If using secrets, or you have a secret file - Specify the password to decrypt them                                                                                             |
| --non-interactive             |       | `ASPIRATE_NON_INTERACTIVE`             | Disables interactive mode for the command                                                                                                                                      |
| --private-registry            |       | `ASPIRATE_PRIVATE_REGISTRY`            | Enables usage of a private registry - which will produce image pull secret.                                                                                                    |
| --private-registry-url        |       | `ASPIRATE_PRIVATE_REGISTRY_URL`        | The url for the private registry                                                                                                                                               |
| --private-registry-username   |       | `ASPIRATE_PRIVATE_REGISTRY_USERNAME`   | The username for the private registry. This is required if passing `--private-registry`.                                                                                       |
| --private-registry-password   |       | `ASPIRATE_PRIVATE_REGISTRY_PASSWORD`   | The password for the private registry. This is required if passing `--private-registry`.                                                                                       |
| --private-registry-email      |       | `ASPIRATE_PRIVATE_REGISTRY_EMAIL`      | The email for the private registry. This is purely optional and will default to `aspirate@aspirate.com`.                                                                       |
| --include-dashboard           |       | `ASPIRATE_INCLUDE_DASHBOARD`           | Boolean flag to specify if the Aspire dashboard should also be included in deployments.                                                                                        |
| --clear-namespace             |       | `ASPIRATE_ALLOW_CLEAR_NAMESPACE`       | Boolean flag to specify the specified namespace should automatically be cleaned during a deployment.                                                                           |
| --launch-profile              | -lp   | 'ASPIRATE_LAUNCH_PROFILE'              | The launch profile to use when building the Aspire Manifest.                                                                                                                   |