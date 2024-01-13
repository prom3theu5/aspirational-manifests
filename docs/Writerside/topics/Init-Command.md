# Initialization

The `init` command allows you to bootstrap certain settings for an aspire project that %product% will use.

- ContainerRegistry: setting this means you do not need one in your csproj, and if it isn't found - all builds will use this.
- ContainerRepositoryPrefix: setting this all container repositories in image names will be prefixed with this value.
- ContainerTag: will override the container tag used if not in your csproj - if not specified in settings, will fall-back to latest.
- ContainerBuilder: this will allow you to set if docker or podman will be used.
- TemplatePath: this customises the path used when loading templates that get transformed to manifests, you can take the templates folder from the source, and modify to your hearts content with all your custom changes, and as long as you don't remove the placeholders, %product% will use those instead of its built in.

## Example

```bash
  aspirate init
```

## Cli Options (Optional)

| Option                        | Alias | Environmental Variable Counterpart     | Description                                                                       |
|-------------------------------|-------|----------------------------------------|-----------------------------------------------------------------------------------|
| --project-path                | -p    | `ASPIRATE_PROJECT_PATH`                | The path to the aspire project.                                                   |
| --container-builder           |       | `ASPIRATE_CONTAINER_BUILDER`           | The Container Builder: can be `docker` or `podman`. The default is `docker`.      |
| --container-registry          | -cr   | `ASPIRATE_CONTAINER_REGISTRY`          | The Container Registry to use as the fall-back value for all containers.          |
| --container-repository-prefix |       | `ASPIRATE_CONTAINER_REPOSITORY_PREFIX` | The Container Repository Prefix to use as the fall-back value for all containers. |
| --container-image-tag         | -ct   | `ASPIRATE_CONTAINER_IMAGE_TAG`         | The Container Image Tag to use as the fall-back value for all containers.         |
| --template-path               | -tp   | `ASPIRATE_TEMPLATE_PATH`               | The path to the templates directory.                                              |
| --non-interactive             |       | `ASPIRATE_NON_INTERACTIVE`             | Disables interactive mode for the command                                         |
| --secret-provider             |       | `ASPIRATE_SECRET_PROVIDER`             | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64` |
| --disable-secrets             |       | `ASPIRATE_DISABLE_SECRETS`             | Disables secrets management features.                                             |
