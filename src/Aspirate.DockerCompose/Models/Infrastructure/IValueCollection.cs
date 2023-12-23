namespace Aspirate.DockerCompose.Models.Infrastructure;

public interface IValueCollection<T> : IValueCollection, ICollection<T> where T : class
{
}

public interface IValueCollection : IEnumerable
{
}
