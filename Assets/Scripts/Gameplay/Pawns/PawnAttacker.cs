using System;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnAttacker : MonoBehaviour
    {
        private IPawnNormal  _pawn;
        private PawnAnimator _pawnAnimator;
        private PawnInventory _pawnInventory;
        
        private IPawnNormal  _target;
        private Action _onAttacked;
        
        public void Init(IPawnNormal  pawn, PawnAnimator pawnAnimator, PawnInventory pawnInventory)
        {
            _pawn = pawn;
            _pawnAnimator = pawnAnimator;
            _pawnInventory = pawnInventory;
        }

        public void AttackTarget(IPawnNormal target, Action onAttacked)
        {
            _target = target;
            _onAttacked = onAttacked;
            _target.Damageable.PreDamage(_pawn, () => _pawnAnimator.AnimateAttack());
        }

        private void OnDamageDealt(int value)
        {
            //_pawn.PawnData.ModifyLevelBy(value);
            _target.Damageable.PostDamage(() => _onAttacked?.Invoke());
        }

        #region Animation Events
       
        public void Hit()
        {
            var damage = _pawn.PawnData.BloodLevel;

            if (_pawnInventory.HasWeapon())
            {
                damage += _pawnInventory.GetWeaponDamageModifier();
            }
            
            _target.Damageable.Damage(damage, OnDamageDealt);
        }
        
        #endregion
    }
}
