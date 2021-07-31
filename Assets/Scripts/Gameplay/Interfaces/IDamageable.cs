using System;
using Gameplay.Сharacters;

namespace Gameplay.Interfaces
{
    public interface IDamageable : IInteractable
    {
        void PreDamage(IDamageable attacker, Action onPreDamage);
        void PostDamage(Action onPostDamage);
        void Damage(int value, Action<int> onDamageDealt);
        bool IsEnemyFor(PawnController pawn);
    }
}
