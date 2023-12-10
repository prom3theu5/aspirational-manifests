# Building Projects and Containers

The Build command will build all projects defined in the aspire manifest file, and push the containers to the registry (if specified), or the local docker daemon.

The command is extremely useful for rebuilding and pushing containers to the registry using a simple menu to select the items you want to build.

The command will first create the manifest file, however, this can be overridden if you pass in the path to an existing manifest file using the `--aspire-manifest` or `-m` flag and supplying the path.

## Example

```bash
  aspirate build
```

```bash
  aspirate build -m ./manifest.json
```

## Cli Options (Optional)

| Option                | Alias | Description                                                                       |
|-----------------------|-------|-----------------------------------------------------------------------------------|
| --project-path        | -p    | The path to the aspire project.                                                   |
| --aspire-manifest     | -m    | The aspire manifest file to use                                                   |
| --container-image-tag | -ct   | The Container Image Tag to use as the fall-back value for all containers.         |
| --container-registry  | -cr   | The Container Registry to use as the fall-back value for all containers.          |
| --container-builder   |       | The Container Builder: can be `docker` or `podman`. The default is `docker`.      |
| --non-interactive     |       | Disables interactive mode for the command                                         |
| --secret-provider     |       | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64` |
| --disable-secrets     |       | Disables secrets management features.                                             |
