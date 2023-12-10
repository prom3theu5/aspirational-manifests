# Applying Manifests to a Cluster

To apply the manifests to your cluster, run:

```bash
aspirate apply
```

You will first be presented with all the context names (unless you have passed one in as a cli option) in your kubeconfig file, and will be asked to select one.
This will be used for deployment

If you have a secret file, you will be prompted to enter the password to decrypt it.

## Cli Options (Optional)

| Option               | Alias | Description                                                                               |
|----------------------|-------|-------------------------------------------------------------------------------------------|
| --input-path         | -i    | The path for the kustomize manifests directory. Defaults to `%output-dir%`                |
| --kube-context       | -k    | The name of the kubernetes context within your kubeconfig to apply / deploy manifests to. |
| --secret-password    |       | If using secrets, or you have a secret file - Specify the password to decrypt them        |
| --non-interactive    |       | Disables interactive mode for the command                                                 |
| --secret-provider    |       | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64`         |
| --disable-secrets    |       | Disables secrets management features.                                                     |