namespace Aspirate.Shared.Models.Process;
public class ProcessWrapper(int id, string fileName, int? parentId = null)
{
    public int Id { get; } = id;
    public string FileName { get; } = fileName;
    public int? ParentId { get; } = parentId;
}
