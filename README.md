![nuget-icon](https://github.com/prom3theu5/aspirational-manifests/assets/1518610/5f4402e9-6f2c-4ca4-b457-206fb8233155)
# Aspirate (Aspir8)

### Automate deployment of a .NET Aspire AppHost to a Kubernetes Cluster

<https://github.com/prom3theu5/aspirational-manifests/assets/1518610/319c4e1e-d47f-40e3-a8c3-ddf124b003a2>

---

# Table of Contents
1. [To Install as a global tool](#to-install-as-a-global-tool)
2. [ContainerRegistry](#containerregistry)
3. [Producing Manifests](#producing-manifests)
4. [Build](#build)
5. [Apply Manifests](#apply-manifests)
6. [Remove Manifests](#remove-manifests)
7. [Non-Interactive Invocation](#non-interactive-invocation)
8. [Uninstall tool](#uninstall-tool)
9. [Configuring the Windows Terminal For Unicode and Emoji Support](#configuring-the-windows-terminal-for-unicode-and-emoji-support)

## To Install as a global tool

```bash
dotnet tool install -g aspirate --prerelease
```

> NOTE: While Aspirate is in development the package will be versioned as a preview and the `--prelease` option will get the latest preview.

---

## ContainerRegistry

You're csproj files (projects) that will be build as containers **MUST** contain ContainerRegistry as a minimum, or the sdk will raise a CONTAINERS1013 error.
To get around this - you can either add it as required, or use the 'init' command.
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

---

## Producing Manifests

Navigate to your Aspire project's AppHost directory, and run:

```bash
aspirate generate
```
This command (by-default) will also build selected projects, and push the containers to the interpolated ContainerRegistry.
Builds can be skipped by passing the `--skip-build` flag.

Your manifests will be in the AppHost/aspirate-output directory by default.

---

## Build

The Build command will build all projects defined in the aspire manifest file, and push the containers to the interpolated ContainerRegistry.

The command is extremely useful for rebuilding and pushing containers to the registry using a simple menu to select the items you want to build.

The command will first create the manifest file, however this can be overridden if you pass in the path to an existing manifest file using the `--aspire-manifest` or `-m` flag and supplying the path.

```bash
aspirate build
```

---

## Apply Manifests

To apply the manifests to your cluster, run:

```bash
aspirate apply
```

Aspirate will first ask you which context they would like you to operate on, and will confirm first that you wish to act.

---

## Remove Manifests

To remove the manifests from your cluster, run:

```bash
aspirate destroy
```

Aspirate will first ask you which context they would like you to operate on, and will confirm first that you wish to act.

---

## Non-Interactive Invocation
All commands can be invoked non-interactively by passing the `--non-interactive` flag.

This will cause the tool to use the default context and not prompt for confirmation.

When using this flag, all configuration arguments must be passed on the command line.

---

## Uninstall tool
Aspirate can be uninstalled as a global tool by running:

```bash
dotnet tool uninstall -g aspirate
```

---

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
