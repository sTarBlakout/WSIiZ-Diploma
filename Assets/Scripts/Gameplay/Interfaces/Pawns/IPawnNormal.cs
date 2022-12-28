using System.Collections.Generic;

namespace Gameplay.Interfaces
{
    public interface IPawnNormal : IPawn
    {
        bool IsAlive { get; }
        IDamageable Damageable { get; }
        new IPawnNormalData PawnData { get; }

        void GiveItems(List<IItem> item);
    }
}
