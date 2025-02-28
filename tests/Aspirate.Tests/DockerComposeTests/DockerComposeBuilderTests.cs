using DockerComposeBuilder.Converters;
using DockerComposeBuilder.Emitters;
using DockerComposeBuilder.Model.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspirate.Tests.DockerComposeTests;

public class DockerComposeBuilderTests
{
    private readonly ISerializer _serializer = new SerializerBuilder()
        .WithTypeConverter(new YamlValueCollectionConverter())
        .WithTypeConverter(new PublishedPortConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithEventEmitter(nextEmitter => new FlowStyleStringSequences(nextEmitter))
        .WithEventEmitter(nextEmitter => new FlowStringEnumConverter(nextEmitter))
        .WithEventEmitter(nextEmitter => new ForceQuotedStringValuesEventEmitter(nextEmitter))
        .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
        .WithNewLine("\n")
        .Build();

    [Fact]
    public async Task SerializeComposeFile_Empty_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithImage_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithPrivillegedService_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .WithPrivileged()
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await  Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithDockerfile_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .WithBuild(x => x
                    .WithDockerfile("a.dockerfile")
                )
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithExposedPort_ShouldBeValid()
    {
        var ports = new List<Port>
        {
            new() { Published = 18888, Target = 18888 },
        };

        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .WithPortMappings(ports.ToArray())
                .WithBuild(x => x
                    .WithDockerfile("a.dockerfile")
                )
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithExposedPorts_ShouldBeValid()
    {
        var ports = new List<Port>
        {
            new() { Published = 18888, Target = 18888 },
            new() { Published = 18889, Target = 18889 },
            new() { Published = 18890, Target = 18890 },
            new() { Published = 18891, Target = 18891 },
            new() { Published = 18891, Target = 18891, Name = "custom-name", Protocol = "tcp" },
            new() { Published = 18891, Target = 18891, Name = "custom-name-two", Protocol = "udp" },
        };

        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .WithPortMappings(ports.ToArray())
                .WithBuild(x => x
                    .WithDockerfile("a.dockerfile")
                )
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }

    [Fact]
    public async Task SerializeComposeFile_WithBuildArguments_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("dotnetaspire/servicea")
                .WithBuild(x => x
                    .WithContext(".")
                    .WithDockerfile("a.dockerfile")
                    .WithArguments(a => a
                        .AddWithoutValue("ENV_1")
                        .Add(new KeyValuePair<string, string>("ENV_2", "value"))
                        .Add(new BuildArgument("ENV_3", "value"))
                    )
                )
                .Build()
            )
            .Build();

        var result = _serializer.Serialize(compose);

        await Verify(result)
            .UseDirectory("VerifyResults");
    }
}
