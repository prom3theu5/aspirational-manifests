# Non-Interactive Invocation

All can be invoked non-interactively by passing the `--non-interactive` flag.

This will cause the tool to use the default context and not prompt for confirmation.

When using this flag, all configuration arguments must be passed on the command line.

`apply` and `destroy` will be invoked non-interactively by default if all required arguments are passed.

## Examples

```bash
aspirate build --non-interactive -m ./manifest.json
```

```bash
aspirate generate --non-interactive -m ./manifest.json --skip-build --secret-password someSecret --image-pull-policy IfNotPresent
```

```bash
aspirate apply --non-interactive --secret-password someSecret -k docker-desktop
or
aspirate apply --secret-password someSecret -k docker-desktop
```

```bash
aspirate destroy --non-interactive -k docker-desktop
or
aspirate destroy -k docker-desktop
```
