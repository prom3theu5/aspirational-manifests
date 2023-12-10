# Secrets File Contents

An example of the contents of a secret file when using the default `Password` provider is shown below:

```json
{
  "salt": "2hO/L8lfSH6BG5J1",
  "hash": "4nLU37zidTC1TF6EM8h6\u002BIz79wIu03nYAIiMhuIKcwU=",
  "secrets": {
    "catalogservice": {
      "ConnectionStrings__catalogdb": "2hO/L8lfSH6BG5J1YKAxbgV8Jkg33lnuKqrPD5/kCk\u002BJZRhJz33KFWZnLIEL2P2Z52M3Nf3K55RUctdzR4rVtovBFtFJLqO4cCDXc2\u002BEleXzyn48vdEOJ37tmU1V0VLGPzFYsGjHV3DQ"
    },
    "basketservice": {
      "ConnectionStrings__basketcache": "2hO/L8lfSH6BG5J1sXfcPV\u002BSi4P4rio4OETGIoXuHVLH",
      "ConnectionStrings__messaging": "2hO/L8lfSH6BG5J1wMfN/sAof7ZZJpQIPWvilJbmCEuOOlhdzn/LBC8tKoEK2tSI4zoxPf3G9NhUcphmT53R4NGQRYE="
    },
    "orderprocessor": {
      "ConnectionStrings__messaging": "2hO/L8lfSH6BG5J1wMfN/sAof7ZZJpQIPWvilJbmCEuOOlhdzn/LBC8tKoEK2tSI4zoxPf3G9NhUcphmT53R4NGQRYE="
    },
    "catalogdbapp": {
      "ConnectionStrings__catalogdb": "2hO/L8lfSH6BG5J1YKAxbgV8Jkg33lnuKqrPD5/kCk\u002BJZRhJz33KFWZnLIEL2P2Z52M3Nf3K55RUctdzR4rVtovBFtFJLqO4cCDXc2\u002BEleXzyn48vdEOJ37tmU1V0VLGPzFYsGjHV3DQ"
    }
  },
  "secretsVersion": 1
}
```

The `salt` and `hash` properties are used to encrypt the secrets in the `secrets` property.

The `secretsVersion` property is used to track the version of the secret file format. Each time the format changes, this value will be incremented.

Each individual secret is encrypted using the `AesGcm` algorithm, using the `salt` and `hash` properties as the key.

The `secrets` property contains a dictionary of secrets, where the key is the name of the service, and the value is a dictionary of secrets for that service.

The key of each secret is the name of the secret, and the value is the encrypted secret.

The `ConnectionStrings__catalogdb` secret is an example of a secret that is used by multiple services. When a manifest is generated for kubernetes, this will
be converted to a secret that is used by multiple deployments.