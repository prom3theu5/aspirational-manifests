# Disable Secret Management

If secrets are not required, the `--disable-secrets` flag can be passed to all commands to disable secret functionality.

However, once manifests are generated with secrets included, they cannot be disabled without regenerating the manifests.

## Example

```bash
aspirate generate --disable-secrets
```