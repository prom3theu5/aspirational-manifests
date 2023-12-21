namespace Aspirate.Shared.Models.Aspirate;

public class CommandAvailableResult
{
    public bool IsAvailable { get; set; }
    public string? FullPath { get; set; }

    public static CommandAvailableResult NotAvailable => new() { IsAvailable = false };
    public static CommandAvailableResult Available(string fullPath) => new() { IsAvailable = true, FullPath = fullPath };
}
