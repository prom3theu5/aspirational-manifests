# Generating Manifests and Secrets

Navigate to your Aspire project's AppHost directory, and run:

```bash
aspirate generate
```
This command (by default) will also build selected projects, and push the containers to the registry (if specified), or the local docker daemon.
Builds can be skipped by passing the `--skip-build` flag.

Your manifests will be in the `%output-dir%` directory by default.

## Cli Options (Optional)

| Option                | Alias | Description                                                                                                                                                                    |
|-----------------------|-------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| --project-path        | -p    | The path to the aspire project.                                                                                                                                                |
| --aspire-manifest     | -m    | The aspire manifest file to use                                                                                                                                                |
| --output-path         | -o    | The path to the output directory. Defaults to `%output-dir%`                                                                                                                   |
| --skip-build          |       | Skips build and Push of containers.                                                                                                                                            |
| --skip-final          | -sf   | Skips The final generation of the kustomize manifest, which is the parent top level file                                                                                       |
| --container-image-tag | -ct   | The Container Image Tag to use as the fall-back value for all containers.                                                                                                      |
| --container-registry  | -cr   | The Container Registry to use as the fall-back value for all containers.                                                                                                       |
| --container-builder   |       | The Container Builder: can be `docker` or `podman`. The default is `docker`.                                                                                                   |
| --image-pull-policy   |       | The image pull policy to use for all containers in generated manifests. Can be `Always`, `Never` or `IfNotPresent`. For your local docker desktop cluster - use `IfNotPresent` |
| --secret-provider     |       | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64`                                                                                              |
| --disable-secrets     |       | Disables secrets management features.                                                                                                                                          |