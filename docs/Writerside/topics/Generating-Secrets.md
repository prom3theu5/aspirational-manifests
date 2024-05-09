# Generating Secrets

The `generate` command is used to generate secrets for the application.
After containers and projects have been build and pushed, if there are any protectable configuration values within the aspire manifest, secret generation will commence.

If you do not already have existing secrets, you will be prompted to enter a password to encrypt the secrets with.
You will have to enter this twice, once to confirm the password.
This password will be used to encrypt the secrets and will be required to decrypt them. Please keep this password safe.

If you already have existing secrets, you will be prompted to enter the password used to encrypt them.
You have three chances to enter the correct password, after which the generation process will be aborted.

If you already have existing secrets, you will then be presented with a menu allowing you to choose from three options:

> **Use Existing**
> 
> The existing secrets will be used and no further action will be taken. The `generate` command will then move on to manifest generation.

> **Augment by adding / replacing values**
>
> %product% will go through each protectable value in the aspire manifest. If a secret already exists, you will get to option to keep the existing value, or replace it with a new one. If a secret does not exist, you will be prompted to replace it with the new value.

> **Overwrite / Create new Password**
> 
> You will be prompted to enter a new password to encrypt the secrets with. This treats the secret state as being empty, and will generate new secrets for all protectable values in the aspire manifest.

After secrets have been generated, they will be encrypted and stored in the `aspirate-state.json` file.
The `generate` command will then move on to manifest generation.