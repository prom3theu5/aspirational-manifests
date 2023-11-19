# Aspirate (Aspir8)
## Convert Aspire Configuration file to Kustomize Manifests for K8s.

Test with:
```bash
dotnet run -- endtoend -m ./Example/aspire-manifest.json -o ./output
cd ./output
kustomize build .
```

Currently just supports Projects as a test - but postgres server and database resources do get deserialized.