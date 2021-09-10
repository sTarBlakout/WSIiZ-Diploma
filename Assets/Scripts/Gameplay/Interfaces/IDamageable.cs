using System;
using Gameplay.Pawns;

namespace Gameplay.Interfaces
{
    public interface IDamageable
    {
        void PreDamage(IPawn attacker, Action onPreDamage);
        void PostDamage(Action onPostDamage);
        void Damage(int value, Action<int> onDamageDealt);
    }
}
