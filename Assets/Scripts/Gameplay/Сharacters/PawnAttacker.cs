using System;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnAttacker : MonoBehaviour
    {
        private PawnData _pawnData;
        private PawnAnimator _pawnAnimator;

        private IDamageable _damageable;
        private Action _onAttacked;
        
        public void Init(PawnData pawnData, PawnAnimator pawnAnimator)
        {
            _pawnData = pawnData;
            _pawnAnimator = pawnAnimator;
        }

        public void AttackTarget(IDamageable damageable, Action onAttacked)
        {
            _damageable = damageable;
            _onAttacked = onAttacked;
            _pawnAnimator.AnimateAttack();
        }

        public void Hit()
        {
            _damageable.Damage(_pawnData.Damage);
            _onAttacked?.Invoke();
        }
    }
}
