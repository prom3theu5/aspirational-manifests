## Run with bindMounts example

```bash
  aspirate run --enable-minikube-mount
```
What it does:
When this flag is passed to the `run` command, we do a combination of what we do in `generate` and `apply`, except we don't actually write anything manifests - we simply apply it directly to the kubernetes cluster, and start the `minikube mount` processes as normal.