# Aspirate (Aspir8)
## Convert Aspire Configuration file to Kustomize Manifests for K8s.

Test with:
```bash
dotnet run -- e2e -p ./Example/AppHost -o ./output
cd ./output
kustomize build .
```
