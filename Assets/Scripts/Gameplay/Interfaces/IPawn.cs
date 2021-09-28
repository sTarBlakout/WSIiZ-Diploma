using Gameplay.Core;

namespace Gameplay.Interfaces
{
    public interface IPawn
    {
        PawnRelation RelationTo(IPawn pawn);
        
        bool IsAlive { get; }
        bool IsBlockingTile { get; }
        IPawnData PawnData { get; }
        IDamageable Damageable { get; }
    }
}
