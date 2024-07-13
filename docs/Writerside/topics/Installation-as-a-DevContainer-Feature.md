# Installation as a DevContainer Feature

## What is a DevContainer?

A DevContainer is a Docker container that is used to develop code in a consistent environment.

## Why use a DevContainer?

A DevContainer provides a consistent environment for all developers working on a project.

This means that all developers will have the same version of tools, such as the Azure CLI, installed.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/)

An IDE that supports DevContainers, such as Visual Studio Code, Visual Studio, or JetBrains Rider.

### Visual Studio Code
- [Visual Studio Code](https://code.visualstudio.com/)
- [Visual Studio Code Remote - Containers Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
- [Visual Studio Code Remote - SSH Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-ssh)
- [Visual Studio Code Remote - WSL Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-wsl)
- [Visual Studio Code Remote - SSH: Editing Configuration Files](https://code.visualstudio.com/docs/remote/ssh#_editing-configuration-files)

### Visual Studio

- [Visual Studio](https://visualstudio.microsoft.com/)
- [Visual Studio Tools for Docker](https://marketplace.visualstudio.com/items?itemName=ms-vsclient.vs-toolsfordocker)
- [Visual Studio Tools for Kubernetes](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools)
- [Visual Studio Tools for Azure Functions](https://marketplace.visualstudio.com/items?itemName=VisualStudioFunctionsTeam.vscode-azurefunctions)
- [Visual Studio Tools for Azure](https://marketplace.visualstudio.com/items?itemName=ms-vscode.vscode-node-azure-pack)

### JetBrains Rider

- [JetBrains Rider](https://www.jetbrains.com/rider/)
- [JetBrains Rider Docker Support](https://www.jetbrains.com/help/rider/Docker.html)
- [JetBrains Rider Kubernetes Support](https://www.jetbrains.com/help/rider/Deploying_Applications_on_Kubernetes.html)

## Using %product% in a DevContainer

```json
features": {
  "ghcr.io/prom3theu5/aspirational-manifests/aspirate:latest": {}
}
```

## Example Dev Container Configuration

```json
{
  "name": ".NET Aspire Backend",
  "image": "mcr.microsoft.com/devcontainers/base:ubuntu",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {},
    "ghcr.io/devcontainers/features/dotnet:2": { "workloads": "aspire" },
    "ghcr.io/devcontainers/features/kubectl-helm-minikube:1": {},
    "ghcr.io/devcontainers/features/github-cli:1": {},
    "ghcr.io/azure/azure-dev/azd:0": { "version": "latest" },
    "ghcr.io/prom3theu5/aspirational-manifests/aspirate:latest": {},
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "redhat.vscode-yaml",
        "streetsidesoftware.code-spell-checker",
        "ms-azuretools.vscode-bicep",
        "eamodio.gitlens",
      ]
    }
  }
}
```
