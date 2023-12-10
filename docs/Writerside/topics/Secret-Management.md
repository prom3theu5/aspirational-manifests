# Secret Management

Aspirate now includes built-in support for robust secret management, allowing you to easily encrypt sensitive data such as connection strings.
This feature is designed to increase security and minimize vulnerabilities.

Aspirate currently supports two secret providers, which can be selected using the command line options `--secret-provider`.

The default provider is `Password`, which uses AesGcm encryption to encrypt the secret's file using a password.
The user supplies this password during the `generate` and `apply` processes.
It's generated using Pbkdf2 with SHA256, one million iterations, and the hash and salt are stored in the secret file.
Secrets protected by this provider are only accessible to users who know the password, and are completely safe to store in a Git repository.

An alternative provider is `Base64`, which uses Base64 encoding to encode the secret within the secret file.
This provider is not recommended for production use, as the secrets are not encrypted and are therefore vulnerable to attack.
However, it's useful for testing and development purposes.
Secrets in this format should not be stored in a Git repository.