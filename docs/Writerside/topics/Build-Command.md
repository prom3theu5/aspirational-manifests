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

| Option                        | Alias | Environmental Variable Counterpart     | Description                                                                                                                                                 |
|-------------------------------|-------|----------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------|
| --project-path                | -p    | `ASPIRATE_PROJECT_PATH`                | The path to the aspire project.                                                                                                                             |
| --aspire-manifest             | -m    | `ASPIRATE_ASPIRE_MANIFEST_PATH`        | The aspire manifest file to use                                                                                                                             |
| --container-image-tag         | -ct   | `ASPIRATE_CONTAINER_IMAGE_TAG`         | The Container Image Tag to use as the fall-back value for all containers.                                                                                   |
| --container-registry          | -cr   | `ASPIRATE_CONTAINER_REGISTRY`          | The Container Registry to use as the fall-back value for all containers.                                                                                    |
| --container-repository-prefix |       | `ASPIRATE_CONTAINER_REPOSITORY_PREFIX` | The Container Repository Prefix to use as the fall-back value for all containers.                                                                           |
| --container-builder           |       | `ASPIRATE_CONTAINER_BUILDER`           | The Container Builder: can be `docker` or `podman`. The default is `docker`.                                                                                |
| --non-interactive             |       | `ASPIRATE_NON_INTERACTIVE`             | Disables interactive mode for the command                                                                                                                   |
| --disable-secrets             |       | `ASPIRATE_DISABLE_SECRETS`             | Disables secrets management features.                                                                                                                       |
| --runtime-identifier          |       | `ASPIRATE_RUNTIME_IDENTIFIER`          | Sets the runtime identifier for project builds. Defaults to `linux-x64`.                                                                                    |
| --compose-build               |       |                                        | Can be included one or more times to set certain dockerfile resource building to be handled by the compose file. This will skip build and push in aspirate. |
| --launch-profile              | -lp   | 'ASPIRATE_LAUNCH_PROFILE'              | The launch profile to use when building the Aspire Manifest.                                                                                                |
| --use-secrets                 |       | 'ASPIRATE_USE_SECRETS'                 | Will unlock the secrets so their values can be used for docker build arguments                                                                              |
| --secret-password             |       | `ASPIRATE_SECRET_PASSWORD`             | If using secrets, or you have a secret file - Specify the password to decrypt them. Required if `--use-secrets` and `--non-interactive` are set             |
