namespace Aspirate.DockerCompose.Models.Infrastructure;

public interface IKeyValue : IKey
{
    string Value { get; set; }
}
