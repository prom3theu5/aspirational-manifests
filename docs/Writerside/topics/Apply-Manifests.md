# Applying Manifests to a Cluster

To apply the manifests to your cluster, run:

```bash
aspirate apply
```

You will first be presented with all the context names (unless you have passed one in as a cli option) in your kubeconfig file, and will be asked to select one.
This will be used for deployment

If you have a secret file, you will be prompted to enter the password to decrypt it.

## Cli Options (Optional)

| Option            | Alias | Environmental Variable Counterpart | Description                                                                               |
|-------------------|-------|------------------------------------|-------------------------------------------------------------------------------------------|
| --input-path      | -i    | `ASPIRATE_INPUT_PATH`              | The path for the kustomize manifests directory. Defaults to `%output-dir%`                |
| --kube-context    | -k    | `ASPIRATE_KUBERNETES_CONTEXT`      | The name of the kubernetes context within your kubeconfig to apply / deploy manifests to. |
| --secret-password |       | `ASPIRATE_SECRET_PASSWORD`         | If using secrets, or you have a secret file - Specify the password to decrypt them        |
| --non-interactive |       | `ASPIRATE_NON_INTERACTIVE`         | Disables interactive mode for the command                                                 |
| --secret-provider |       | `ASPIRATE_SECRET_PROVIDER`         | The secret provider to use. Defaults to `Password`. Can be `Password` or `Base64`         |
| --disable-secrets |       | `ASPIRATE_DISABLE_SECRETS`         | Disables secrets management features.                                                     | 