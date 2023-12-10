# Installing as a Global Tool

> **Note**
>
> While %product% is in development, the package will be versioned as a preview and the `--prelease` option will get the latest preview.
>
{style="note"}

%product% is shipped to nuget as a .NET Core Global Tool, which means you can install it with a single command:

```bash
dotnet tool install -g aspirate --prerelease
```

Alternatively, if you already have %product% installed, you can update it to the latest version using the following command:

```bash
dotnet tool update -g aspirate --prerelease
```