using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspirate.Shared.Interfaces.Commands.Contracts;
public interface IMinikubeOptions
{
    bool? DisableMinikubeMountAction { get; set; }
}
