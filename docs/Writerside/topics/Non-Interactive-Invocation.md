# Non-Interactive Invocation

All commands apart from `generate` can be invoked non-interactively by passing the `--non-interactive` flag.
Generate can be used if secrets are disabled.

This will cause the tool to use the default context and not prompt for confirmation.

When using this flag, all configuration arguments must be passed on the command line.

## Example

```bash
aspirate build --non-interactive -m ./manifest.json
```
