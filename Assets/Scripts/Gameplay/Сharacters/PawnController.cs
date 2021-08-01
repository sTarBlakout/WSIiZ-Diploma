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
        
        [SerializeField] private PawnHealthIndicator pawnHealthIndicator;
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnAttacker pawnAttacker;
        [SerializeField] private PawnMover pawnMover;
        
        [SerializeField] private GameObject pawnGraphics;
        [SerializeField] private Collider pawnCollider;

        private PawnData _currPawnData;

        public void Init()
        {
            _currPawnData = Instantiate(pawnData);

            pawnMover.Init(_currPawnData, pawnAnimator, pawnGraphics);
            pawnAttacker.Init(_currPawnData, pawnAnimator, this);
            pawnHealthIndicator.Init(_currPawnData);
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
        
        public bool IsInteractable()
        {
            return _currPawnData.Level != 0;
        }
        
        public bool IsEnemyFor(PawnController pawn)
        {
            return pawnData.TeamId != pawn.pawnData.TeamId;
        }
        
        public void PreDamage(IDamageable attacker, Action onPrepared)
        {
            onPrepared += TryBlock;
            RotateTo(attacker.Position, onPrepared);
        }

        public void Damage(int value, Action<int> onDamageDealt)
        {
            var health = _currPawnData.Level;
            var dmgReceived = value;
            _currPawnData.ModifyLevelBy(-value);
            if (_currPawnData.Level == 0) dmgReceived = value + (health - value);
            onDamageDealt?.Invoke(dmgReceived);
        }
        
        public void PostDamage(Action onPostDamage)
        {
            StartCoroutine(PostDamageCoroutine(onPostDamage));
        }

        #endregion

        private IEnumerator PostDamageCoroutine(Action onPostDamage)
        {
            pawnAnimator.AnimateGetHit();
            if (_currPawnData.Level == 0) StartCoroutine(DeathCoroutine());
            yield return new WaitForSeconds(pawnData.AfterDamageDelay);
            pawnAnimator.AnimateBlock(false);
            onPostDamage?.Invoke();
        }

        private void TryBlock()
        {
            pawnAnimator.AnimateBlock(true);
        }

        private IEnumerator DeathCoroutine()
        {
            pawnAnimator.AnimateDie();
            pawnCollider.enabled = false;
            pawnMover.enabled = false;
            yield return new WaitForSeconds(1f);
            pawnHealthIndicator.Show(false);
        }
    }
}
