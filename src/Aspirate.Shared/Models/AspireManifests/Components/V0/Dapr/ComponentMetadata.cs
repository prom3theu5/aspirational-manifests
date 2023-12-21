namespace Aspirate.Shared.Models.AspireManifests.Components.V0.Dapr;

public class Metadata
{
    [JsonPropertyName("application")]
    public string Application { get; set; } = default!;

    [JsonPropertyName("appId")]
    public string AppId { get; set; } = default!;

    [JsonPropertyName("components")]
    public List<string>? Components { get; set; }

    [JsonPropertyName("appChannelAddress")]
    public string? AppChannelAddress { get; init; }

    [JsonPropertyName("appHealthCheckPath")]
    public string? AppHealthCheckPath { get; init; }

    [JsonPropertyName("appHealthProbeInterval")]
    public int? AppHealthProbeInterval { get; init; }

    [JsonPropertyName("appHealthProbeTimeout")]
    public int? AppHealthProbeTimeout { get; init; }

    [JsonPropertyName("appHealthThreshold")]
    public int? AppHealthThreshold { get; init; }

    [JsonPropertyName("appMaxConcurrency")]
    public int? AppMaxConcurrency { get; init; }

    [JsonPropertyName("appPort")]
    public int? AppPort { get; init; }

    [JsonPropertyName("appProtocol")]
    public string? AppProtocol { get; init; }

    [JsonPropertyName("command")]
    public IImmutableList<string> Command { get; init; } = ImmutableList<string>.Empty;

    [JsonPropertyName("config")]
    public string? Config { get; init; }

    [JsonPropertyName("daprGrpcPort")]
    public int? DaprGrpcPort { get; init; }

    [JsonPropertyName("daprHttpMaxRequestSize")]
    public int? DaprHttpMaxRequestSize { get; init; }

    [JsonPropertyName("daprHttpPort")]
    public int? DaprHttpPort { get; init; }

    [JsonPropertyName("daprHttpReadBufferSize")]
    public int? DaprHttpReadBufferSize { get; init; }

    [JsonPropertyName("daprInternalGrpcPort")]
    public int? DaprInternalGrpcPort { get; init; }

    [JsonPropertyName("daprListenAddresses")]
    public string? DaprListenAddresses { get; init; }

    [JsonPropertyName("enableApiLogging")]
    public bool? EnableApiLogging { get; init; }

    [JsonPropertyName("enableAppHealthCheck")]
    public bool? EnableAppHealthCheck { get; init; }

    [JsonPropertyName("enableProfiling")]
    public bool? EnableProfiling { get; init; }

    [JsonPropertyName("logLevel")]
    public string? LogLevel { get; init; }

    [JsonPropertyName("metricsPort")]
    public int? MetricsPort { get; init; }

    [JsonPropertyName("placementHostAddress")]
    public string? PlacementHostAddress { get; init; }

    [JsonPropertyName("profilePort")]
    public int? ProfilePort { get; init; }

    [JsonPropertyName("resourcePaths")]
    public IImmutableSet<string> ResourcesPaths { get; init; } = ImmutableHashSet<string>.Empty;

    [JsonPropertyName("runFile")]
    public string? RunFile { get; init; }

    [JsonPropertyName("runtimePath")]
    public string? RuntimePath { get; init; }

    [JsonPropertyName("unixDomainSocket")]
    public string? UnixDomainSocket { get; init; }
}
