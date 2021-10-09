using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawnData
    {
        int TeamId { get; }
        int Damage { get; }
        int DistancePerTurn { get; }
        int ActionsPerTurn { get; }
        int AttackDistance { get; }
        GameObject WayMoveLinePrefab { get; }
        GameObject WayAttackLinePrefab { get; }
        GameObject WayInteractLinePrefab { get; }
        void ModifyLevelBy(int value);
    }
}
