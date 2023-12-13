# Generating Manifests and Secrets

Navigate to your Aspire project's AppHost directory, and run:

```bash
aspirate generate
```
This command (by default) will also build selected projects, and push the containers to the registry (if specified), or the local docker daemon.
Builds can be skipped by passing the `--skip-build` flag.

Your manifests will be in the `%output-dir%` directory by default.

## Cli Options (Optional)

| Option                | Alias | Environmental Variable Counterpart         | Description                                                                                                                                                                    |
|-----------------------|-------|--------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| --project-path        | -p    | `ASPIRATE_PROJECT_PATH`                    | The path to the aspire project.                                                                                                                                                |
| --aspire-manifest     | -m    | `ASPIRATE_ASPIRE_MANIFEST_PATH`            | The aspire manifest file to use                                                                                                                                                |
| --output-path         | -o    | `ASPIRATE_OUTPUT_PATH`                     | The path to the output directory. Defaults to `%output-dir%`                                                                                                                   |
| --skip-build          |       | `ASPIRATE_SKIP_BUILD`                      | Skips build and Push of containers.                                                                                                                                            |
| --skip-final          | -sf   | `ASPIRATE_SKIP_FINAL_KUSTOMIZE_GENERATION` | Skips The final generation of the kustomize manifest, which is the parent top level file                                                                                       |
| --container-image-tag | -ct   | `ASPIRATE_CONTAINER_IMAGE_TAG`             | The Container Image Tag to use as the fall-back value for all containers.                                                                                                      |
| --container-registry  | -cr   | `ASPIRATE_CONTAINER_REGISTRY`              | The Container Registry to use as the fall-back value for all containers.                                                                                                       |
| --container-builder   |       | `ASPIRATE_CONTAINER_BUILDER`               | The Container Builder: can be `docker` or `podman`. The default is `docker`.                                                                                                   |
| --image-pull-policy   |       | `ASPIRATE_IMAGE_PULL_POLICY`               | The image pull policy to use for all containers in generated manifests. Can be `Always`, `Never` or `IfNotPresent`. For your local docker desktop cluster - use `IfNotPresent` |
| --secret-provider     |       | `ASPIRATE_SECRET_PROVIDER`                 | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64`                                                                                              |
| --disable-secrets     |       | `ASPIRATE_DISABLE_SECRETS`                 | Disables secrets management features.                                                                                                                                          |