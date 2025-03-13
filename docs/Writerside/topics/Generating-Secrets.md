# Generating Secrets

The `generate` command is used to generate secrets for the application.
After containers and projects have been build and pushed, if there are any protectable configuration values within the aspire manifest, secret generation will commence.

If you do not already have existing secrets, you will be prompted to enter a password to encrypt the secrets with.
You will have to enter this twice, once to confirm the password.
This password will be used to encrypt the secrets, and parameter resources and will be required to decrypt them. Please keep this password safe.

If you already have existing secrets, you will be prompted to enter the password used to encrypt them.
You have three chances to enter the correct password, after which the generation process will be aborted.

If you already have existing secrets, they will be reused unless `--replace-secrets` is passed as a cli option.

After secrets have been generated, they will be encrypted and stored in the `aspirate-state.json` file.
The `generate` command will then move on to manifest generation.