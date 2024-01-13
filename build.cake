#addin "nuget:?package=Cake.MinVer&version=3.0.0"

using System.Text.Json;

var target = Argument("Target", "CI");

var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration", "Release");

var shouldPack = HasArgument("pack") ? Argument<bool>("pack") : false;

var settings = new MinVerSettings()
{
    AutoIncrement = MinVerAutoIncrement.Minor,
    DefaultPreReleasePhase = "preview",
    MinimumMajorMinor = "0.1",
    TagPrefix = "v",
    Verbosity = MinVerVerbosity.Trace,
};

var version = MinVer(settings);

Task("Clean")
    .Description("Cleans the artifacts, bin and obj directories.")
    .Does(() =>
    {
        DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings() { Force = true, Recursive = true });
        DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings() { Force = true, Recursive = true });
        CleanDirectory("./artifacts");
        CleanDirectory("./test-output");
    });

Task("Restore")
    .Description("Restores NuGet packages.")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetRestore();
    });

Task("Build")
    .Description("Builds the solution.")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(
            ".",
            new DotNetBuildSettings()
            {
                Configuration = configuration,
                NoRestore = true,
                ArgumentCustomization = args =>
                  args
                  .Append($"-p:Version={version}")
                  .Append($"-p:InformationalVersion={version}"),
            });
    });

Task("Test")
    .Description("Runs unit tests and outputs test results to the artifacts directory.")
    .Does(() =>
    {
        DotNetTest(
            "tests/Aspirate.Tests/Aspirate.Tests.csproj",
            new DotNetTestSettings()
            {
                Blame = true,
                Configuration = configuration,
                ResultsDirectory = "./test-output",
                NoBuild = true,
                NoRestore = true,
                Collectors = new string[] { "Code Coverage", "XPlat Code Coverage" },
            });
    });

Task("Pack")
    .Description("Packs the Required Project")
    .IsDependentOn("Test")
    .WithCriteria(shouldPack)
    .Does(() =>
        {
            DotNetPack("src/Aspirate.Cli/Aspirate.Cli.csproj",
             new DotNetPackSettings()
                        {
                            NoBuild = true,
                            NoRestore = true,
                            NoLogo = true,
                            OutputDirectory = "./artifacts",
                            Verbosity = DotNetVerbosity.Minimal,
                            Configuration = configuration,
                            ArgumentCustomization = builder => builder.Append($"-p:PackageVersion={version}")
                        });
        });

Task("CI")
    .Description("Cleans, restores NuGet packages, builds the solution and then runs unit tests.")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

Task("LocalTestInstall")
    .Description("Installs the tool locally")
    .IsDependentOn("Build")
    .Does(() =>
    {
        StartProcess("dotnet", new ProcessSettings()
        {
            Arguments = $"tool uninstall -g aspirate",
        });
        
        DotNetPack("src/Aspirate.Cli/Aspirate.Cli.csproj",
            new DotNetPackSettings()
                    {
                        NoBuild = true,
                        NoRestore = true,
                        NoLogo = true,
                        OutputDirectory = "./artifacts",
                        Verbosity = DotNetVerbosity.Minimal,
                        Configuration = configuration,
                        ArgumentCustomization = builder => builder.Append($"-p:PackageVersion=999.99.99")
                    });

        var package = GetFiles("./artifacts/*.nupkg").FirstOrDefault();
        if (package == null)
        {
            throw new Exception("Could not find package");
        }

        StartProcess("dotnet", new ProcessSettings()
        {
            Arguments = $"tool install -g --add-source ./artifacts aspirate",
        });
    });


RunTarget(target);
