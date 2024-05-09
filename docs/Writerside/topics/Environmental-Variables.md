# Environmental Variables

All Cli options can also be set using environmental variables.
Utilising environmental variables can be useful when using %product% in a CI/CD pipeline.

An example of using %product% with environmental variables is shown below:

```bash
export ASPIRATE_OUTPUT_DIR=/home/user/output
export ASPIRATE_MANIFEST_FILE=/home/user/manifest.yaml
aspirate generate
```

All environmental variables are prefixed with `ASPIRATE_`.

The following is a selection of environmental variables are available (Each of the command pages lists all possible env vars for a command): 

| Name                                       | Description                                                                 | Default Value     |
|--------------------------------------------|-----------------------------------------------------------------------------|-------------------|
| `ASPIRATE_NO_LOGO`                         | Hides the %product% logo when executing                                     |                   |
| `ASPIRATE_CONTAINER_BUILDER`               | The Container Builder: can be 'docker' or 'podman'. The default is 'docker' | `docker`          |
| `ASPIRATE_CONTAINER_IMAGE_TAG`             | The tag to use for the container image.                                     | `latest`          |
| `ASPIRATE_CONTAINER_REGISTRY`              | The container registry to use.                                              |                   |
| `ASPIRATE_DISABLE_SECRETS`                 | Disables secrets generation.                                                | `false`           |
| `ASPIRATE_IMAGE_PULL_POLICY`               | The image pull policy to use.                                               |                   |
| `ASPIRATE_INPUT_PATH`                      | The path to the input directory.                                            | `%output-dir%`    |
| `ASPIRATE_KUBERNETES_CONTEXT`              | The kubernetes context to use.                                              |                   |
| `ASPIRATE_NON_INTERACTIVE`                 | Disables interactive prompts.                                               | `false`           |
| `ASPIRATE_OUTPUT_PATH`                     | The path to the output directory.                                           | `%output-dir%`    |
| `ASPIRATE_PROJECT_PATH`                    | The path to the project directory.                                          | `.`               |
| `ASPIRATE_SECRET_PASSWORD`                 | The password to use for secrets decryption.                                 |                   |
| `ASPIRATE_SKIP_BUILD`                      | Skips the build process.                                                    | `false`           |
| `ASPIRATE_SKIP_FINAL_KUSTOMIZE_GENERATION` | Skips the final kustomize generation process.                               | `false`           |
| `ASPIRATE_TEMPLATE_PATH`                   | The path to the template directory.                                         | ``                |
| `ASPIRATE_ASPIRE_MANIFEST_PATH`            | The path to the aspire manifest file.                                       | `./manifest.json` |
