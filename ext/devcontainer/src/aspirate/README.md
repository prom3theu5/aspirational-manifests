# Aspir8 (aspirate)

Installs [Aspir8](https://github.com/prom3theu5/aspirational-manifests)

> **NOTE**
>
> Requires `ghcr.io/devcontainers/features/dotnet:2` or a base image with dotnet installed.

## Example Usage - Install current latest `aspirate` version **without dotnet base image**

```json
"features": {
  "ghcr.io/devcontainers/features/dotnet:2": {}
  "ghcr.io/vdboots/aspirational-manifests/aspirate:latest": {}
}
```

## Example Usage - Install current latest `aspirate` version **with dotnet base image**

```json
"features": {
  "ghcr.io/vdboots/aspirational-manifests/aspirate:latest": {}
}
```
