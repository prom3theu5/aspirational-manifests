namespace Aspirate.Tests.ServiceTests;

public class ContainerCompositionServiceTest
{
    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public async Task BuildAndPushContainerForProject_ShouldCallExpectedMethods_WhenCalled(string builder)
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var project = new ProjectResource
        {
            Path = "testPath",
        };

        var containerDetails = new MsBuildContainerProperties();

        projectPropertyService.GetProjectPropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(
                JsonSerializer.Serialize(
                    new MsBuildProperties<MsBuildPublishingProperties>
                    {
                        Properties = new()
                        {
                            PublishSingleFile = "true",
                            PublishTrimmed = "true",
                        },
                    }));

        var response = builder == "docker" ? DockerInfoOutput : PodmanInfoOutput;

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, response, string.Empty, 0)));

        shellExecutionService.IsCommandAvailable(Arg.Any<string>())
            .Returns(Task.FromResult(CommandAvailableResult.Available(builder)));

        // Act
        var result = await service.BuildAndPushContainerForProject(project, containerDetails, builder);

        // Assert
        await shellExecutionService.Received(2).ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public async Task BuildAndPushContainerForProject_ShouldThrowWhenBuilderOffline(string builder)
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var project = new ProjectResource
        {
            Path = "testPath",
        };

        var containerDetails = new MsBuildContainerProperties();

        projectPropertyService.GetProjectPropertiesAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(
                JsonSerializer.Serialize(
                    new MsBuildProperties<MsBuildPublishingProperties>
                    {
                        Properties = new()
                        {
                            PublishSingleFile = "true",
                            PublishTrimmed = "true",
                        },
                    }));

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, "test", string.Empty, 0)));

        shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(Arg.Any<string>(), Arg.Any<ArgumentsBuilder>(),Arg.Any<Dictionary<string, string?>>())
            .Returns(Task.FromResult(false));

        // Act
        var action = () => service.BuildAndPushContainerForProject(project, containerDetails, builder);

        // Assert
        await action.Should().ThrowAsync<ActionCausesExitException>();
    }


    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public async Task BuildAndPushContainerForDockerfile_ShouldCallExpectedMethods_WhenCalled(string builder)
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var dockerfile = new DockerfileResource { Path = "testPath", Context = "testContext" };
        var imageName = "testImageName";
        var registry = "testRegistry";

        var response = builder == "docker" ? DockerInfoOutput : PodmanInfoOutput;

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, response, string.Empty, 0)));

        shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(Arg.Any<string>(), Arg.Any<ArgumentsBuilder>(),Arg.Any<Dictionary<string, string?>>())
            .Returns(Task.FromResult(true));

        shellExecutionService.IsCommandAvailable(Arg.Any<string>())
            .Returns(Task.FromResult(CommandAvailableResult.Available(builder)));

        // Act
        var result = await service.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, true);

        // Assert
        await shellExecutionService.Received(1).IsCommandAvailable(Arg.Any<string>());
        await shellExecutionService.Received(3).ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null));
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public async Task BuildAndPushContainerForDockerfile_ShouldSetEnvVarsAsBuildArgs_WhenCalled(string builder)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile("./testDockerfile", string.Empty);
        var console = new TestConsole();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var dockerfile = new DockerfileResource
        {
            Path = "./testDockerfile",
            Context = "testContext",
            Env = new()
            {
                ["TestArg"] = "TestValue",
                ["TestArgTwo"] = "TestValueTwo",
            },
        };

        var response = builder == "docker" ? DockerInfoOutput : PodmanInfoOutput;

        shellExecutionService.ExecuteCommand(Arg.Is<ShellCommandOptions>(options => options.Command != null && options.ArgumentsBuilder != null))
            .Returns(Task.FromResult(new ShellCommandResult(true, response, string.Empty, 0)));

        shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(Arg.Any<string>(), Arg.Any<ArgumentsBuilder>(),Arg.Any<Dictionary<string, string?>>())
            .Returns(Task.FromResult(true));

        shellExecutionService.IsCommandAvailable(Arg.Any<string>())
            .Returns(Task.FromResult(CommandAvailableResult.Available(builder)));

        // Act
        await service.BuildAndPushContainerForDockerfile(dockerfile, builder, "testImageName", "testRegistry", true);

        // Assert
        var testFile = fileSystem.Path.GetFullPath("./testDockerfile");
        var calls = shellExecutionService.ReceivedCalls().ToArray();
        calls.Length.Should().Be(4);

        var buildCall = calls[2];
        VerifyDockerCall(buildCall, $"build --tag \"testregistry/testimagename:latest\" --build-arg TestArg=\"TestValue\" --build-arg TestArgTwo=\"TestValueTwo\" --file \"{testFile}\" testContext");

        var pushCall = calls[3];
        VerifyDockerCall(pushCall, "push testregistry/testimagename:latest");
    }

    [Theory]
    [InlineData("docker")]
    [InlineData("podman")]
    public async Task BuildAndPushContainerForDockerfile_BuilderOffline_ThrowsExitException(string builder)
    {
        // Arrange
        var fileSystem = Substitute.For<IFileSystem>();
        var console = Substitute.For<IAnsiConsole>();
        var projectPropertyService = Substitute.For<IProjectPropertyService>();
        var shellExecutionService = Substitute.For<IShellExecutionService>();

        var service = new ContainerCompositionService(fileSystem, console, projectPropertyService, shellExecutionService);

        var dockerfile = new DockerfileResource { Path = "testPath", Context = "testContext" };
        var imageName = "testImageName";
        var registry = "testRegistry";

        shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(Arg.Any<string>(), Arg.Any<ArgumentsBuilder>(),Arg.Any<Dictionary<string, string?>>())
            .Returns(Task.FromResult(false));

        // Act
        var action = () => service.BuildAndPushContainerForDockerfile(dockerfile, builder, imageName, registry, true);

        // Assert
        await action.Should().ThrowAsync<ActionCausesExitException>();
    }

   private static void VerifyDockerCall(ICall call, string expectedArgumentsOutput)
    {
        if (call.GetArguments()[0] is not ShellCommandOptions options)
        {
            throw new InvalidOperationException("The shell execution service was not called with the expected arguments.");
        }

        options.Should().NotBeNull();
        options.ArgumentsBuilder.RenderArguments().Should().Be(expectedArgumentsOutput);
    }

    private static string DockerInfoOutput =>
        """
        {"ID":"b0f37fbe-6588-473d-aaa1-e06d79ac53d8","Containers":19,"ContainersRunning":19,"ContainersPaused":0,"ContainersStopped":0,"Images":10,"Driver":"overlay2","DriverStatus":[["Backing Filesystem","extfs"],["Supports d_type","true"],["Using metacopy","false"],["Native Overlay Diff","true"],["userxattr","false"]],"Plugins":{"Volume":["local"],"Network":["bridge","host","ipvlan","macvlan","null","overlay"],"Authorization":null,"Log":["awslogs","fluentd","gcplogs","gelf","journald","json-file","local","logentries","splunk","syslog"]},"MemoryLimit":true,"SwapLimit":true,"CpuCfsPeriod":true,"CpuCfsQuota":true,"CPUShares":true,"CPUSet":true,"PidsLimit":true,"IPv4Forwarding":true,"BridgeNfIptables":true,"BridgeNfIp6tables":true,"Debug":false,"NFd":162,"OomKillDisable":false,"NGoroutines":168,"SystemTime":"2023-12-21T16:40:46.714070722Z","LoggingDriver":"json-file","CgroupDriver":"cgroupfs","CgroupVersion":"2","NEventsListener":9,"KernelVersion":"6.4.16-linuxkit","OperatingSystem":"Docker Desktop","OSVersion":"","OSType":"linux","Architecture":"aarch64","IndexServerAddress":"https://index.docker.io/v1/","RegistryConfig":{"AllowNondistributableArtifactsCIDRs":null,"AllowNondistributableArtifactsHostnames":null,"InsecureRegistryCIDRs":["127.0.0.0/8"],"IndexConfigs":{"docker.io":{"Name":"docker.io","Mirrors":[],"Secure":true,"Official":true},"hubproxy.docker.internal:5555":{"Name":"hubproxy.docker.internal:5555","Mirrors":[],"Secure":false,"Official":false}},"Mirrors":null},"NCPU":10,"MemTotal":8225546240,"GenericResources":null,"DockerRootDir":"/var/lib/docker","HttpProxy":"http.docker.internal:3128","HttpsProxy":"http.docker.internal:3128","NoProxy":"hubproxy.docker.internal","Name":"linuxkit-1edf7766b1ac","Labels":[],"ExperimentalBuild":false,"ServerVersion":"24.0.6","Runtimes":{"io.containerd.runc.v2":{"path":"runc"},"runc":{"path":"runc"}},"DefaultRuntime":"runc","Swarm":{"NodeID":"","NodeAddr":"","LocalNodeState":"inactive","ControlAvailable":false,"Error":"","RemoteManagers":null},"LiveRestoreEnabled":false,"Isolation":"","InitBinary":"docker-init","ContainerdCommit":{"ID":"8165feabfdfe38c65b599c4993d227328c231fca","Expected":"8165feabfdfe38c65b599c4993d227328c231fca"},"RuncCommit":{"ID":"v1.1.8-0-g82f18fe","Expected":"v1.1.8-0-g82f18fe"},"InitCommit":{"ID":"de40ad0","Expected":"de40ad0"},"SecurityOptions":["name=seccomp,profile=unconfined","name=cgroupns"],"Warnings":["WARNING: daemon is not using the default seccomp profile"],"ClientInfo":{"Debug":false,"Version":"24.0.6","GitCommit":"ed223bc","GoVersion":"go1.20.7","Os":"darwin","Arch":"arm64","BuildTime":"Mon Sep  4 12:28:49 2023","Context":"desktop-linux","Plugins":[{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v0.11.2-desktop.5","ShortDescription":"Docker Buildx","Name":"buildx","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-buildx"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v2.23.0-desktop.1","ShortDescription":"Docker Compose","Name":"compose","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-compose"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v0.1.0","ShortDescription":"Docker Dev Environments","Name":"dev","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-dev"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v0.2.20","ShortDescription":"Manages Docker extensions","Name":"extension","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-extension"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v0.1.0-beta.9","ShortDescription":"Creates Docker-related starter files for your project","Name":"init","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-init"},{"SchemaVersion":"0.1.0","Vendor":"Anchore Inc.","Version":"0.6.0","ShortDescription":"View the packaged-based Software Bill Of Materials (SBOM) for an image","URL":"https://github.com/docker/sbom-cli-plugin","Name":"sbom","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-sbom"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v0.26.0","ShortDescription":"Docker Scan","Name":"scan","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-scan"},{"SchemaVersion":"0.1.0","Vendor":"Docker Inc.","Version":"v1.0.9","ShortDescription":"Docker Scout","Name":"scout","Path":"/Users/prom3theu5/.docker/cli-plugins/docker-scout"}],"Warnings":null}}
        """;

    private static string PodmanInfoOutput =>
        """
        {
          "host": {
            "arch": "amd64",
            "buildahVersion": "1.23.0",
            "cgroupManager": "systemd",
            "cgroupVersion": "v2",
            "cgroupControllers": [],
            "conmon": {
              "package": "conmon-2.0.29-2.fc34.x86_64",
              "path": "/usr/bin/conmon",
              "version": "conmon version 2.0.29, commit: "
            },
            "cpus": 8,
            "distribution": {
              "distribution": "fedora",
              "version": "34"
            },
            "eventLogger": "journald",
            "hostname": "localhost.localdomain",
            "idMappings": {
              "gidmap": [
        	{
        	  "container_id": 0,
        	  "host_id": 3267,
        	  "size": 1
        	},
        	{
        	  "container_id": 1,
        	  "host_id": 100000,
        	  "size": 65536
        	}
              ],
              "uidmap": [
        	{
        	  "container_id": 0,
        	  "host_id": 3267,
        	  "size": 1
        	},
        	{
        	  "container_id": 1,
        	  "host_id": 100000,
        	  "size": 65536
        	}
              ]
            },
            "kernel": "5.13.13-200.fc34.x86_64",
            "logDriver": "journald",
            "memFree": 1785753600,
            "memTotal": 16401895424,
            "networkBackend": "cni",
            "networkBackendInfo": {
              "backend": "cni",
              "package": "containernetworking-plugins-1.0.1-1.fc34.x86_64\npodman-plugins-3.4.4-1.fc34.x86_64",
              "path": "/usr/libexec/cni",
              "dns": {
                "version": "CNI dnsname plugin\nversion: 1.3.1\ncommit: unknown",
                "package": "podman-plugins-3.4.4-1.fc34.x86_64",
                "path": "/usr/libexec/cni/dnsname"
              }
            },
            "ociRuntime": {
              "name": "crun",
              "package": "crun-1.0-1.fc34.x86_64",
              "path": "/usr/bin/crun",
              "version": "crun version 1.0\ncommit: 139dc6971e2f1d931af520188763e984d6cdfbf8\nspec: 1.0.0\n+SYSTEMD +SELINUX +APPARMOR +CAP +SECCOMP +EBPF +CRIU +YAJL"
            },
            "os": "linux",
            "remoteSocket": {
              "path": "/run/user/3267/podman/podman.sock"
            },
            "serviceIsRemote": false,
            "security": {
              "apparmorEnabled": false,
              "capabilities": "CAP_CHOWN,CAP_DAC_OVERRIDE,CAP_FOWNER,CAP_FSETID,CAP_KILL,CAP_NET_BIND_SERVICE,CAP_SETFCAP,CAP_SETGID,CAP_SETPCAP,CAP_SETUID",
              "rootless": true,
              "seccompEnabled": true,
              "seccompProfilePath": "/usr/share/containers/seccomp.json",
              "selinuxEnabled": true
            },
            "slirp4netns": {
              "executable": "/bin/slirp4netns",
              "package": "slirp4netns-1.1.12-2.fc34.x86_64",
              "version": "slirp4netns version 1.1.12\ncommit: 7a104a101aa3278a2152351a082a6df71f57c9a3\nlibslirp: 4.4.0\nSLIRP_CONFIG_VERSION_MAX: 3\nlibseccomp: 2.5.0"
            },
            "pasta": {
              "executable": "/usr/bin/passt",
              "package": "passt-0^20221116.gace074c-1.fc34.x86_64",
              "version": "passt 0^20221116.gace074c-1.fc34.x86_64\nCopyright Red Hat\nGNU Affero GPL version 3 or later \u003chttps://www.gnu.org/licenses/agpl-3.0.html\u003e\nThis is free software: you are free to change and redistribute it.\nThere is NO WARRANTY, to the extent permitted by law.\n"
            },
            "swapFree": 15687475200,
            "swapTotal": 16886259712,
            "uptime": "47h 17m 29.75s (Approximately 1.96 days)",
            "linkmode": "dynamic"
          },
          "store": {
            "configFile": "/home/dwalsh/.config/containers/storage.conf",
            "containerStore": {
              "number": 9,
              "paused": 0,
              "running": 1,
              "stopped": 8
            },
            "graphDriverName": "overlay",
            "graphOptions": {

            },
            "graphRoot": "/home/dwalsh/.local/share/containers/storage",
            "graphStatus": {
              "Backing Filesystem": "extfs",
              "Native Overlay Diff": "true",
              "Supports d_type": "true",
              "Using metacopy": "false"
            },
            "imageCopyTmpDir": "/home/dwalsh/.local/share/containers/storage/tmp",
            "imageStore": {
              "number": 5
            },
            "runRoot": "/run/user/3267/containers",
            "volumePath": "/home/dwalsh/.local/share/containers/storage/volumes",
            "transientStore": false
          },
          "registries": {
            "search": [
          "registry.fedoraproject.org",
          "registry.access.redhat.com",
          "docker.io",
          "quay.io"
        ]
          },
          "plugins": {
            "volume": [
              "local"
            ],
            "network": [
              "bridge",
              "macvlan"
            ],
            "log": [
              "k8s-file",
              "none",
              "journald"
            ]
          },
          "version": {
            "APIVersion": "4.0.0",
            "Version": "4.0.0",
            "GoVersion": "go1.16.6",
            "GitCommit": "23677f92dd83e96d2bc8f0acb611865fb8b1a56d",
            "BuiltTime": "Tue Sep 14 15:45:22 2021",
            "Built": 1631648722,
            "OsArch": "linux/amd64"
          }
        }
        """;
}
