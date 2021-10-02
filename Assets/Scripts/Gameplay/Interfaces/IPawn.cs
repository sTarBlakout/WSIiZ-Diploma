using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawn
    {
        PawnRelation RelationTo(IPawn pawn);
        
        bool IsAlive { get; }
        bool IsBlockingTile { get; }
        Vector3 WorldPosition { get; }
        IPawnData PawnData { get; }
        IDamageable Damageable { get; }
    }
}
