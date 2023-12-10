# Initialization

The `init` command allows you to bootstrap certain settings for an aspire project that %product% will use.

- ContainerRegistry: setting this means you do not need one in your csproj, and if it isn't found - all builds will use this.
- ContainerTag - will override the container tag used if not in your csproj - if not specified in settings, will fall-back to latest.
- TemplatePath - this customises the path used when loading templates that get transformed to manifests, you can take the templates folder from the source, and modify to your hearts content with all your custom changes, and as long as you don't remove the placeholders, %product% will use those instead of its built in.

## Example

```bash
  aspirate init
```

## Cli Options (Optional)

| Option                | Alias | Description                                                                       |
|-----------------------|-------|-----------------------------------------------------------------------------------|
| --project-path        | -p    | The path to the aspire project.                                                   |
| --container-registry  | -cr   | The Container Registry to use as the fall-back value for all containers.          |
| --container-image-tag | -ct   | The Container Image Tag to use as the fall-back value for all containers.         |
| --template-path       | -tp   | The path to the templates directory.                                              |
| --non-interactive     |       | Disables interactive mode for the command                                         |
| --secret-provider     |       | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64` |
| --disable-secrets     |       | Disables secrets management features.                                             |
