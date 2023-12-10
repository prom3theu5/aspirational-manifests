# Removing Manifests from a Cluster

To remove the manifests from your cluster, run:

```bash
aspirate destroy
```

You will first be presented with all the context names (unless you have passed one in as a cli option) in your kubeconfig file, and will be asked to select one.
This will be used for removal.

If you have a secret file, these secrets will be removed as well. This command does not prompt for a password, as secrets do not need to be decrypted to be removed from your cluster.

## Cli Options (Optional)

| Option               | Alias | Description                                                                               |
|----------------------|-------|-------------------------------------------------------------------------------------------|
| --input-path         | -i    | The path for the kustomize manifests directory. Defaults to `%output-dir%`                |
| --kube-context       | -k    | The name of the kubernetes context within your kubeconfig to apply / deploy manifests to. |
| --non-interactive    |       | Disables interactive mode for the command                                                 |
| --secret-provider    |       | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64`         |
| --disable-secrets    |       | Disables secrets management features.                                                     |