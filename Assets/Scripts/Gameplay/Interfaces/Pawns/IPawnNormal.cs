using System.Collections.Generic;
using Gameplay.Core;

namespace Gameplay.Interfaces
{
    public interface IPawnNormal : IPawn
    {
        bool IsAlive { get; }
        IDamageable Damageable { get; }
        new IPawnNormalData PawnData { get; }

        void GiveItems(List<IItem> item);
        List<(IItem, bool)> GetInventoryItems(ItemType type);
    }
}
