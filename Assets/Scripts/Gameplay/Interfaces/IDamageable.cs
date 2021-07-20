using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IDamageable
    {
        bool IsEnemyFor(PawnController pawn);
        Vector3 Position { get; }
    }
}
