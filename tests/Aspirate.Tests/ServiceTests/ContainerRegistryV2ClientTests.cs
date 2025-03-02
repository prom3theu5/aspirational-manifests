using System.Reflection;

namespace Aspirate.Tests.ServiceTests;
public class ContainerRegistryV2ClientTests
{
    public class SubstitutedHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            SendAsyncCore(request, cancellationToken);

        public virtual Task<HttpResponseMessage> SendAsyncCore(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }

    public class SubstitutedHttpClientBuilder
    {
        private readonly SubstitutedHttpMessageHandler _handler = Substitute.ForPartsOf<SubstitutedHttpMessageHandler>();

        public SubstitutedHttpClientBuilder AddHttpExchange(
            string requestUri,
            Func<CallInfo, HttpResponseMessage> createHttpResponse) =>
            AddHttpExchange(new Uri(requestUri), createHttpResponse);

        public SubstitutedHttpClientBuilder AddHttpExchange(
            Uri requestUri,
            Func<CallInfo, HttpResponseMessage> createHttpResponse)
        {
            _handler
                .SendAsyncCore(
                    Arg.Is<HttpRequestMessage>(x => x.RequestUri! == requestUri),
                    Arg.Any<CancellationToken>())
                .Returns(createHttpResponse);

            return this;
        }

        public HttpClient Build() => new(_handler);
    }

    private static HttpClient BuildSubstitutedHttpClient() =>
        new SubstitutedHttpClientBuilder()
            .AddHttpExchange(
                "http://test/v2/_catalog",
                _ =>
                {
                    var resp = new HttpResponseMessage
                    {
                        Content = new StringContent(
                            /*lang=json,strict*/
                            """
                            {
                                "repositories": [
                                    "headless"
                                ]
                            }
                            """)
                    };

                    resp.Content.Headers.ContentType = new("application/json", "utf-8");

                    return resp;
                })
            .AddHttpExchange(
                "http://test/v2/headless/tags/list",
                _ =>
                {
                    var resp = new HttpResponseMessage
                    {
                        Content = new StringContent(
                            /*lang=json,strict*/
                            """
                            {
                                "name": "headless",
                                "tags": [
                                    "1.2.3-alpha2"
                                ]
                            }
                            """)
                    };

                    resp.Content.Headers.ContentType = new("application/json", "utf-8");

                    return resp;
                })
            .AddHttpExchange(
                new Uri("http://test/v2/headless/manifests/1.2.3-alpha2"),
                _ =>
                {
                    var resp = new HttpResponseMessage
                    {
                        Content = new StringContent(
                            /*lang=json,strict*/
                            """
                            {
                                "schemaVersion": 2,
                                "mediaType": "application/vnd.docker.distribution.manifest.v2\u002Bjson",
                                "config": {
                                    "mediaType": "application/vnd.docker.container.image.v1\u002Bjson",
                                    "size": 3016,
                                    "digest": "sha256:727c8878fed3a91e240ca89ab5b177ec7887b8e094982289ea78240776c358b8"
                                },
                                "layers": [
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 28219301,
                                        "digest": "sha256:7cf63256a31a4cc44f6defe8e1af95363aee5fa75f30a248d95cae684f87c53c"
                                    },
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 18723012,
                                        "digest": "sha256:c0061e74b3e3a349a186353aba7c567f8a6837da04d0b540190617712bd981e4"
                                    },
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 3277,
                                        "digest": "sha256:b4027bb0a9ca1e3baf2f8c95ea5ffb97861623adf783d2259beee5c6ebb4ed5b"
                                    },
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 32242337,
                                        "digest": "sha256:d43efb33ab765bd3cac8cd3358439150a95fd5a8f1adedeafefab3e7982b1159"
                                    },
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 155,
                                        "digest": "sha256:457ab9651207ea490ad92fbfa524d82600d03f1c1bf01de90958b7090f1bdd22"
                                    },
                                    {
                                        "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
                                        "size": 40082,
                                        "digest": "sha256:d4186a6ba6ec53c3e2ff5da080551faa74f84e807e7f74712bfcf3c325bf61d5"
                                    }
                                ]
                            }
                            """)
                    };

                    resp.Content.Headers.ContentType = new("application/vnd.docker.distribution.manifest.v2+json");

                    return resp;
                })
            .AddHttpExchange(
                new Uri("http://test/v2/headless/blobs/sha256:727c8878fed3a91e240ca89ab5b177ec7887b8e094982289ea78240776c358b8"),
                _ =>
                {
                    var resp = new HttpResponseMessage
                    {
                        Content = new StringContent(
                            /*lang=json,strict*/
                            """
                            {
                                "config": {
                                    "ExposedPorts": {
                                        "8080/tcp": {}
                                    },
                                    "Labels": {
                                        "org.opencontainers.image.created": "2025-03-01T00:48:56.4547719Z",
                                        "org.opencontainers.artifact.created": "2025-03-01T00:48:56.4547719Z",
                                        "org.opencontainers.image.authors": "Headless",
                                        "org.opencontainers.image.version": "1.0.0",
                                        "org.opencontainers.image.base.name": "mcr.microsoft.com/dotnet/runtime:8.0",
                                        "net.dot.runtime.majorminor": "8.0",
                                        "net.dot.sdk.version": "9.0.200",
                                        "org.opencontainers.image.base.digest": "sha256:2fd1964f1e5430fc09d756ba49674f0c0c4dfd9a8ed8eccdac27d6df8b718f91"
                                    },
                                    "Env": [
                                        "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin",
                                        "APP_UID=1654",
                                        "ASPNETCORE_HTTP_PORTS=8080",
                                        "DOTNET_RUNNING_IN_CONTAINER=true",
                                        "DOTNET_VERSION=8.0.13"
                                    ],
                                    "WorkingDir": "/app/",
                                    "Entrypoint": [
                                        "dotnet",
                                        "/app/Headless.dll"
                                    ],
                                    "User": "1654"
                                },
                                "created": "2025-03-01T00:48:57.1016665Z",
                                "rootfs": {
                                    "type": "layers",
                                    "diff_ids": [
                                        "sha256:5f1ee22ffb5e68686db3dcb6584eb1c73b5570615b0f14fabb070b96117e351d",
                                        "sha256:13538303ed9cedfa748390cad44880ed7002ff88467c351388a8df331a989662",
                                        "sha256:05df6742558bf5c12ca2109e48499d5d5103127803e51f85502e1b223ce0df00",
                                        "sha256:517b3236c9826dcfbec826d532473c9f944b195b879e0db0fedb0bc8c71bf7d6",
                                        "sha256:ef956b5e5fbcb3087030643987be84696a8a192ebc2f12f2bbe368bc813f8a90",
                                        "sha256:b3bdc1aee0ff8ddb76f7ec0d1d2c1d23080b85fa111f3073a772e2c8c7163cd6"
                                    ]
                                },
                                "architecture": "amd64",
                                "os": "linux",
                                "history": [
                                    {
                                        "comment": "debuerreotype 0.15",
                                        "created": "2025-02-24T00:00:00.0000000Z",
                                        "created_by": "# debian.sh --arch \u0027amd64\u0027 out/ \u0027bookworm\u0027 \u0027@1740355200\u0027"
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:47.0581404Z",
                                        "created_by": "ENV APP_UID=1654 ASPNETCORE_HTTP_PORTS=8080 DOTNET_RUNNING_IN_CONTAINER=true",
                                        "empty_layer": true
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:47.0581404Z",
                                        "created_by": "RUN /bin/sh -c apt-get update     \u0026\u0026 apt-get install -y --no-install-recommends         ca-certificates                 libc6         libgcc-s1         libicu72         libssl3libstdc\u002B\u002B6         tzdata         zlib1g     \u0026\u0026 rm -rf /var/lib/apt/lists/* # buildkit"
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:48.4064609Z",
                                        "created_by": "RUN /bin/sh -c groupadd         --gid=$APP_UID         app     \u0026\u0026 useradd -l         --uid=$APP_UID         --gid=$APP_UID         --create-home         app # buildkit"
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:54.9372187Z",
                                        "created_by": "ENV DOTNET_VERSION=8.0.13",
                                        "empty_layer": true
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:54.9372187Z",
                                        "created_by": "COPY /dotnet /usr/share/dotnet # buildkit"
                                    },
                                    {
                                        "comment": "buildkit.dockerfile.v0",
                                        "created": "2025-02-25T04:26:55.9057482Z",
                                        "created_by": "RUN /bin/sh -c ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet # buildkit"
                                    },
                                    {
                                        "author": ".NET SDK",
                                        "created": "2025-03-01T00:48:57.1016581Z",
                                        "created_by": ".NET SDK Container Tooling, version 9.0.200-rtm.25073.12\u002B90e8b202f25b7c2bf3b883d421ad5b1cb477e8b0"
                                    }
                                ]
                            }
                            """)
                    };

                    resp.Content.Headers.ContentType = new("application/octet-stream");

                    return resp;
                })
            .Build();

    private readonly Lazy<ContainerRegistryV2Client> _lazyContainerRegistryClient = new(() =>
    {
        var client = BuildSubstitutedHttpClient();
        client.BaseAddress = new("http://test/v2/");

        var containerRegistryV2ClientCtor = typeof(ContainerRegistryV2Client).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            [typeof(HttpClient), typeof(bool)])!;

        return (ContainerRegistryV2Client)containerRegistryV2ClientCtor.Invoke([client, false]);
    });

    private ContainerRegistryV2Client ContainerRegistryClient => _lazyContainerRegistryClient.Value;

    [Fact]
    public async Task GetCatalog_CatalogShouldContainHeadless()
    {
        // Arrange

        // Act
        var catalog = await ContainerRegistryClient.GetCatalogAsync();

        // Assert
        Assert.NotNull(catalog);
        Assert.NotNull(catalog.Repositories);
        Assert.Single(catalog.Repositories);
        Assert.Equal("headless", catalog.Repositories.Single());
    }

    [Fact]
    public async Task GetTagList_TagsShouldContain_1_2_3_alpha2()
    {
        // Arrange
        var catalog = await ContainerRegistryClient.GetCatalogAsync();
        var repository = catalog.Repositories.Single();

        // Act
        var tagList = await ContainerRegistryClient.GetTagsAsync(repository);


        // Assert
        Assert.NotNull(tagList);
        Assert.NotNull(tagList.Name);
        Assert.Equal("headless", tagList.Name);
        Assert.NotNull(tagList.Tags);
        Assert.Single(tagList.Tags);
        Assert.Equal("1.2.3-alpha2", tagList.Tags.Single());
    }

    [Fact]
    public async Task GetManifest_ManifestShouldContainConfig()
    {
        // Arrange
        var catalog = await ContainerRegistryClient.GetCatalogAsync();
        var repository = catalog.Repositories.Single();
        var tagList = await ContainerRegistryClient.GetTagsAsync(repository);
        var tag = tagList.Tags.Single();

        // Act
        var manifest = await ContainerRegistryClient.GetManifestAsync(repository, tag);


        // Assert
        Assert.NotNull(manifest);
        Assert.NotNull(manifest.Config);
        Assert.NotNull(manifest.Config.Digest);
        Assert.NotNull(manifest.Config.MediaType);
        Assert.Equal("application/vnd.docker.container.image.v1+json", manifest.Config.MediaType);
        Assert.Equal(3016, manifest.Config.Size);
        Assert.Equal("sha256:727c8878fed3a91e240ca89ab5b177ec7887b8e094982289ea78240776c358b8", manifest.Config.Digest);
    }

    [Fact]
    public async Task GetDockerImageJsonBlob_BlobShouldHaveCreatedDate()
    {
        // Arrange
        var catalog = await ContainerRegistryClient.GetCatalogAsync();
        var repository = catalog.Repositories.Single();
        var tagList = await ContainerRegistryClient.GetTagsAsync(repository);
        var tag = tagList.Tags.Single();
        var manifest = await ContainerRegistryClient.GetManifestAsync(repository, tag);

        // Act
        var dockerImage = await ContainerRegistryClient.GetDockerImageJsonBlobAsync(repository, manifest.Config.Digest);

        // Assert
        Assert.NotNull(dockerImage);
        Assert.Equal(2025, dockerImage.Created.Year);
        Assert.Equal(3, dockerImage.Created.Month);
        Assert.Equal(1, dockerImage.Created.Day);
        Assert.Equal(0, dockerImage.Created.Hour);
        Assert.Equal(48, dockerImage.Created.Minute);
        Assert.Equal(57, dockerImage.Created.Second);
        Assert.Equal(101, dockerImage.Created.Millisecond);
        Assert.Equal(666, dockerImage.Created.Microsecond);
    }
}
