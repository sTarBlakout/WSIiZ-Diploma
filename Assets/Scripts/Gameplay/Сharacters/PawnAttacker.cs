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
        
        private IDamageable _targetDamageable;
        private Action _onAttacked;
        
        public void Init(PawnData pawnData, PawnAnimator pawnAnimator, IDamageable damageable)
        {
            _pawnData = pawnData;
            _pawnAnimator = pawnAnimator;
            _damageable = damageable;
        }

        public void AttackTarget(IDamageable damageable, Action onAttacked)
        {
            _targetDamageable = damageable;
            _onAttacked = onAttacked;
            _targetDamageable.PrepareForDamage(_damageable, () => _pawnAnimator.AnimateAttack());
        }

        #region Unused Animation Events
       
        public void Hit()
        {
            _targetDamageable.Damage(_pawnData.Damage, () => { });
            _onAttacked?.Invoke();
        }
        
        #endregion
    }
}
