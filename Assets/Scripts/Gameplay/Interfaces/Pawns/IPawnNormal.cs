using System.Collections.Generic;
using Gameplay.Core;

namespace Gameplay.Interfaces
{
    public interface IPawnNormal : IPawn
    {
        bool IsAlive { get; }
        IDamageable Damageable { get; }
        IInventory Inventory { get; }
        new IPawnNormalData PawnData { get; }
    }
}
