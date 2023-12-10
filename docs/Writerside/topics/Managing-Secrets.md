# Managing Secrets

During the `generate` and `apply` processes, you will be prompted to input a password.
This password is used to encrypt your secrets in the secret file, named `%secrets-file%`, located in the `%output-dir%` directory.

> **Note**
> 
> This password is not stored anywhere, and is only used to encrypt and decrypt the secrets file.
> If you lose this password, you will be unable to access your secrets and will need to use the `generate` command to create a new one.
{style="warning"}
