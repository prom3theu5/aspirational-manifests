## 🚀 Minikube Mounts

Aspirate now includes built-in support for automating **Minikube bind mount processes**, making it easy to mount container resources that use bind mounts, by passing one of the following flags:
`-em` or `--enable-minikube-mount`

When using **Kubernetes on a local Minikube cluster**, volume mounting requires a kind of _middle-man_ — something that bridges your **container’s filesystem** with a **path on your host machine**.

Here’s how it works:

- You specify a path like `/mount/path` in your pod so the container knows where to read or write data.
- Minikube then mounts a **local host folder** to that same path inside the cluster.
- This bridge allows your pod to access local files as if they were inside the container.

This new functionality makes it so you DONT have to do any of the above manually, as aspirate now handles it automatically, for all container resources in your Aspire manifest that has bind mounts.