## Applying kustomize manifests with bindMounts example

```bash
  aspirate apply --enable-minikube-mount
```
What it does:
When this flag is passed to the `apply` command, and `KubeContext` is set to `minikube`, we iterate through each of the bind mount paths that were previously stored in `aspirate-state.json` during `generate`, and run `minikube mount {source}:{target}` for all of them.
We then take the process Id we get in return, and stored it back into `aspirate-state.json` so we know which process Ids to kill later.

During this action, we also do a 60 second delay BEFORE we start applying manifests to the cluster. This is to ensure that all the `minikube mount` processes are actually fully up and running, which can sometimes take up to a minute, so they are ready
before the containers actually get deployed.