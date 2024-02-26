namespace Aspirate.Tests.DockerComposeTests;

public class DockerComposeBuilderTests
{
    [Fact]
    public async Task SerializeComposeFile_Empty_ShouldBeValid()
    {
        var compose = Builder.MakeCompose()
            .Build();

        var result = compose.Serialize();

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

        var result = compose.Serialize();

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

        var result = compose.Serialize();

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

        var result = compose.Serialize();

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

        var result = compose.Serialize();

        await Verify(result)
            .UseDirectory("VerifyResults");
    }
}
