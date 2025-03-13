# Managing Secrets

During the `generate` and `apply` processes, you will be prompted to input a password.
This password is used to encrypt your secrets in the state file, named aspirate-state.json

> **Note**
> 
> This password is not stored anywhere, and is only used to encrypt and decrypt the secrets within the state file.
> If you lose this password, you will be unable to access your secrets and will need to use the `generate` command to create a new one.
{style="warning"}
