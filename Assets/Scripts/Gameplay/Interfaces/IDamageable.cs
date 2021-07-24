using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IDamageable
    {
        void Damage(int value);
        bool IsEnemyFor(PawnController pawn);
        Vector3 Position { get; }
    }
}
