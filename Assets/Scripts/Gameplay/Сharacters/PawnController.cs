using System;
using System.Collections.Generic;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnController : MonoBehaviour, IDamageable
    {
        [SerializeField] private PawnData pawnData;
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnMover pawnMover;
        [SerializeField] private PawnAttacker pawnAttacker;
        [SerializeField] private GameObject charGraphics;

        private void Start()
        {
            pawnMover.Init(pawnData, pawnAnimator, charGraphics);
            pawnAttacker.Init(pawnData, pawnAnimator, this);
        }

        public void MovePath(List<Vector3> path, Action onReachedDestination)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else

                pawnMover.MovePath(path, onReachedDestination);
        }
        
        public void RotateTo(Vector3 position, Action onRotated)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else
                pawnMover.RotateTo(position, onRotated);
        }

        public void AttackTarget(IDamageable damageable, Action onAttacked)
        {
            if (pawnAttacker == null)
                Debug.LogError($"{gameObject.name} does not have any attacker component!");
            else
                pawnAttacker.AttackTarget(damageable, onAttacked);
        }

        #region IDamageable Implementation

        public Vector3 Position => transform.position;
        
        public void PrepareForDamage(IDamageable attacker, Action onPrepared)
        {
            onPrepared += TryBlock;
            RotateTo(attacker.Position, onPrepared);
        }

        // TODO: Add some callback and wait a bit
        public void Damage(int value)
        {
            pawnAnimator.AnimateGetHit();
            //pawnAnimator.AnimateBlock(false);
        }

        public bool IsEnemyFor(PawnController pawn)
        {
            return pawnData.TeamId != pawn.pawnData.TeamId;
        }
        
        #endregion
        
        private void TryBlock()
        {
            pawnAnimator.AnimateBlock(true);
        }
    }
}
