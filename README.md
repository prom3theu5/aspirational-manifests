![nuget-icon](https://github.com/prom3theu5/aspirational-manifests/assets/1518610/5f4402e9-6f2c-4ca4-b457-206fb8233155)
# Aspirate (Aspir8)

## note: this project is forked from prom3theu5 and if maintainer continues to maintain the project, this fork will be deleted.

### Handle deployment yaml generation for a .NET Aspire AppHost

<https://github.com/vdboots/aspirational-manifests/assets/1518610/319c4e1e-d47f-40e3-a8c3-ddf124b003a2>

Documentation: https://vdboots.github.io/aspirational-manifests/

# Table of Contents
1. [Installation as a global tool](#to-install-as-a-global-tool)
2. [Producing Manifests](#producing-manifests)
3. [Build](#build)
4. [Secrets Management](#secrets-management)
    - [Managing Secrets](#managing-secrets)
    - [Secrets File Contents](#secrets-file-contents)
    - [Generating Secrets](#generating-secrets)
    - [Applying Secrets](#applying-secrets)
    - [Disabling Secret Management](#disabling-secret-management)
5. [Apply Manifests](#apply-manifests)
6. [Remove Manifests](#remove-manifests)
7. [Init Command](#init)
8. [Non-Interactive Invocation](#non-interactive-invocation)
9. [Uninstall tool](#uninstall-tool)
10. [Configuring the Windows Terminal For Unicode and Emoji Support](#configuring-the-windows-terminal-for-unicode-and-emoji-support)
11. [DevContainer Support](#devcontainer-support)

## To Install as a global tool

```bash
dotnet tool install -g aspirate
```

> NOTE: Add the `--prelease` option, to install the latest preview version.

## Producing Manifests

Navigate to your Aspire project's AppHost directory, and run:

```bash
aspirate generate
```
This command (by-default) will also build selected projects, and push the containers to the interpolated ContainerRegistry.
Builds can be skipped by passing the `--skip-build` flag.

Your manifests will be in the AppHost/aspirate-output directory by default.

If you'd like - you can generate a slim docker-compose deployment instead of a kubernetes deployment.
For this you can pass --output-format compose.
Please note - this will disable secret support.

```bash
aspirate generate --output-format compose
```

## Build

The Build command will build all projects defined in the aspire manifest file, and push the containers to the interpolated ContainerRegistry.

The command is extremely useful for rebuilding and pushing containers to the registry using a simple menu to select the items you want to build.

The command will first create the manifest file, however this can be overridden if you pass in the path to an existing manifest file using the `--aspire-manifest` or `-m` flag and supplying the path.

```bash
aspirate build
```

## Secrets Management

Aspirate now includes built-in support for robust secret management, allowing you to easily encrypt sensitive data such as connection strings. This feature is designed to increase security and minimize vulnerabilities.

### Managing Secrets

During the `generate` and `apply` processes, you will be prompted to input a password.
This password is used to encrypt your secrets in the secrets file, named `aspirate-secrets.json`, located in the `aspirate-output` directory.

### Secrets File Contents

The secrets file contains your encrypted secrets and is safe to commit to your Git repository.
However, please handle this password with care. If it's lost, you'll be unable to access your encrypted secrets and you will need to use the `generate` command to create a new one.

### Generating Secrets

The `generate` command provides an interactive menu during the generation process that allows you to use existing secrets, overwrite them, or update them.

### Applying Secrets

The `apply` command will decrypt the secrets file and apply the secrets to the cluster, along with all the manifests.

### Disabling Secret Management

If secrets are not required, the `--disable-secrets` flag can be passed to all commands to disable secret functionality.
However, once manifests are generated with secrets included, they cannot be disabled without regenerating the manifests.

## Apply Manifests

To apply the manifests to your cluster, run:

```bash
aspirate apply
```

Aspirate will first ask you which context they would like you to operate on, and will confirm first that you wish to act.

## Remove Manifests

To remove the manifests from your cluster, run:

```bash
aspirate destroy
```

Aspirate will first ask you which context they would like you to operate on, and will confirm first that you wish to act.

## Init
The init command allows you to bootstrap certain settings for an aspire project that Aspir8 will use.

- ContainerRegistry: setting this means you do not need one in your csproj, and if it isn't found - all builds will use this.
- ContainerTag - will override the container tag used if not in your csproj - if not specified in settings, will fall-back to latest.
- TemplatePath - this customises the path used when loading templates that get transformed to manifests, you can take the templates folder from the source, and modify to your hearts content with all your custom changes, and as long as you don't remove the placeholders, aspirate will use those instead of its built in.
  More on this and possible use cases (such as adding jobs to create databases etc) when we have docs....

To use the init command, you simply run:

```bash
aspirate init
```

from within your AppHost directory - and it'll ask you which settings you'd like to override.

## Non-Interactive Invocation
All commands apart from generate can be invoked non-interactively by passing the `--non-interactive` flag.
Generate can be used if secrets are disabled.

This will cause the tool to use the default context and not prompt for confirmation.

When using this flag, all configuration arguments must be passed on the command line.

## Uninstall tool
Aspirate can be uninstalled as a global tool by running:

```bash
dotnet tool uninstall -g aspirate
```

## Configuring the Windows Terminal For Unicode and Emoji Support

Windows Terminal supports Unicode and Emoji. However, the shells such as Powershell and cmd.exe do not.
For the difference between the two,
see [What's the difference between a console,
a terminal and a shell](https://www.hanselman.com/blog/whats-the-difference-between-a-console-a-terminal-and-a-shell).

For PowerShell, the following command will enable Unicode and Emoji support. You can add this to your `profile.ps1`
file:

```powershell
[console]::InputEncoding = [console]::OutputEncoding = [System.Text.UTF8Encoding]::new()
```

For cmd.exe, the following steps are required to enable Unicode and Emoji support.

1. Run `intl.cpl`.
2. Click the Administrative tab
3. Click the Change system locale button.
4. Check the "Use Unicode UTF-8 for worldwide language support" checkbox.
5. Reboot.

You will also need to ensure that your Console application is configured to use a font that supports Unicode and Emoji,
such as Cascadia Code.

## DevContainer Support

Aspirate can be used in a devcontainer by installing the feature:

```json
features": {
  "ghcr.io/vdboots/aspirational-manifests/aspirate:latest": {}
}
```

An Example of a devcontainer can be found on the documentation page: [Here](https://prom3theu5.github.io/aspirational-manifests/installation-as-a-devcontainer-feature.html#example-dev-container-configuration)
