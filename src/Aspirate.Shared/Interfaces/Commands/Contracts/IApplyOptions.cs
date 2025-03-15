namespace Aspirate.Shared.Interfaces.Commands.Contracts;

public interface IApplyOptions
{
    bool? RollingRestart { get; set; }
    //bool? DisableMinikubeMountAction { get; set; }
}
