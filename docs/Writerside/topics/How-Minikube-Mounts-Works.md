## How minikube mounts work, and what aspirate does to automate it

Take this example deployment.yaml:

```yaml
volumeMounts:
  - name: myBindMount
    mountPath: /my/target/path
    readOnly: false

terminationGracePeriodSeconds: 180

volumes:
  - name: myBindMount
    hostPath:
      path: /my/source/path
      type: DirectoryOrCreate
```

In this setup

- The container mounts /my/target/path (via volumeMounts)
- Kubernetes maps that to /my/source/path (via hostPath)
- Finally, Minikube needs to mount your real host path (e.g., C:/Users/You/certificates) to /my/source/path

So the full path flow looks like this:
```bash
[container] /my/target/path 
  → [node] /my/source/path 
  → [host]  C:/Users/You/certificates
```
Without this Minikube middle-man, the container has no way to access your actual host file system.

##How aspirate automates this:
- Automatically adds volumeMounts and volumes to your deployment.yaml based on your bind mount definitions,
- Automatically starts minikube mount {hostPath}:{mountPath} processes when needed
- Automatically stops those mount processes when no longer required (e.g., during destroy or stop)

This functionality is used in the following three commmands - `aspirate generate` - `aspirate apply` - `aspirate destroy` - `aspirate run` - `aspirate stop`. 