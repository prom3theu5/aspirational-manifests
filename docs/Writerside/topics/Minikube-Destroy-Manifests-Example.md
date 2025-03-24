## Destroy kustomize manifests with bindMounts example

```bash
  aspirate destroy --enable-minikube-mount
```
What it does:
When this flag is passed to the `destroy` command, OR if restoring state, and this flag was previously set to true, this loops through the bind mounts stored in `aspirate-state.json` and kills the processes based on the process Ids stored.
Additionally, it also find the minikube internal .mount-process file, and removes the process Ids from that file, that we started. If there are no more process Ids left in the file after we are done removing them, we delete the file as it's no longer necessary.

NOTE: Sometimes, minikube does not create the .mount-process file. In that case, we don't try to remove anything from it, since we won't be able to find it. This is not a problem.