# Aspirate (Aspir8)
### Convert Aspire Configuration file to Kustomize Manifests for K8s.

Test with:
```bash
dotnet run -- e2e -p ./Example/AppHost -o ./output
```

---
### To Install as a global tool, until this hits release phase

```bash
gh repo clone prom3theu5/aspirational-manifests
cd aspirational-manifests
dotnet pack -c Release -o ./artifacts
dotnet tool install -g aspirate --add-source ./artifacts/
```

Now navigate to your Aspire project's AppHost directory, and run:
```bash
aspirate e2e -p . -o ./output
```
Your manifests will be in the AppHost/output directory

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
