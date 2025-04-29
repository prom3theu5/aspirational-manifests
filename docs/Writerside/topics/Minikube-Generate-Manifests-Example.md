## Generating kustomize manifests with bindMounts example

```bash
  aspirate generate --enable-minikube-mount
```

What it does:
When this flag is passed to the `generate` command, if `minikube` is set as the kubecontext and output format is `kustomzie`, it goes through all components specified that has bind mounts (ie: they are an IResourceWithBindMounts) - iterates through
all the bind mounts each component has, and stores the unique bind mounts to `aspirate-state.json`. The reason for this is that when we run `aspirate apply` later, we need to be able to find them again, before we start up the `minikube mount` processes.

If `-em` or `--enable-minikube-mount` are not passed, we still write the bind mounts specified to `deployment.yaml`, now the source is not going to be the middle-man path, but instead the real source that was specified in the AppHost project.
We also don't store the mount paths to `aspirate-state.json` in that case, because we don't intend to start any `minikube mount` processes.