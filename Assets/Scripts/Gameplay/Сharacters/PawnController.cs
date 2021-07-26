using System;
using System.Collections;
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

        private int _currHealth;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _currHealth = pawnData.Health;
            
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
        
        public void Damage(int value, Action onDamageDealt)
        {
            pawnAnimator.AnimateGetHit();
            _currHealth = Mathf.Max(0, _currHealth - value);
            if (_currHealth == 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(ProcessPostDamage(onDamageDealt));
            }
        }

        public bool IsEnemyFor(PawnController pawn)
        {
            return pawnData.TeamId != pawn.pawnData.TeamId;
        }
        
        #endregion

        private IEnumerator ProcessPostDamage(Action onDamageDealt)
        {
            yield return new WaitForSeconds(pawnData.AfterDamageDelay);
            pawnAnimator.AnimateBlock(false);
            onDamageDealt?.Invoke();
        }
        
        private void TryBlock()
        {
            pawnAnimator.AnimateBlock(true);
        }

        private void Die()
        {
            pawnAnimator.AnimateDie();
        }
    }
}
