# Aspirate (Aspir8)
### Automate deployment of an Aspire AppHost to a Kubernetes Cluster.

---
### To Install as a global tool

```bash
dotnet tool install -g aspirate
```

---

https://github.com/prom3theu5/aspirational-manifests/assets/1518610/319c4e1e-d47f-40e3-a8c3-ddf124b003a2

### Generating Manifests (apply)
#### ContainerRegistry
You're csproj files (projects) that will be build as containers **MUST** contain ContainerRegistry as a minimum, or the sdk will raise a CONTAINERS1013 error.
To get around this - you can either add it as required, or use the 'init' command.
The init command allows you to bootstrap certain settings for an asire project that Aspir8 will use.

- ContainerRegistry: setting this means you do not need one in your csproj, and if it isn't found - all builds will use this.
- ContainerTag - will override the container tag used if not in your csproj - if not specified in settings, will fall-back to latest.
- TemplatePath - this customises the path used when loading templates that get transformed to manifests, you can take the templates folder from the source, and modify to your hearts content with all your custom changes, and as long as you don't remove the placeholders, aspirate will use those instead of its built in.
  More on this and possible use cases (such as adding jobs to create databases etc) when we have docs....

To use the init command, you simply run:
```bash
aspirate init
```
from withinn your AppHost directory - and it'll ask you which settings you'd like to override.

#### Produce Manifests
Navigate to your Aspire project's AppHost directory, and run:
```bash
aspirate generate
```
Your manifests will be in the AppHost/aspirate-output directory

---
> **_INFORMATION:_**  Both the following commands will first ask you which context they would like you to operate on, and will confirm first that you wish to act.

#### Apply Manifests
To apply the manifests to your cluster, run:
```bash
aspirate apply
```

#### Remove Manifests
To remove the manifests from your cluster, run:
```bash
aspirate destroy
```

---
### Uninstall tool
```bash
dotnet tool uninstall -g aspirate
```

---
### Configuring the Windows Terminal For Unicode and Emoji Support

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
