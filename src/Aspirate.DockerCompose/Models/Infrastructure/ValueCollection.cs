namespace Aspirate.DockerCompose.Models.Infrastructure;

public class ValueCollection<T>(IEnumerable<T> collection) : List<T>(collection), IValueCollection<T>
    where T : class;
