using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnController : MonoBehaviour, IPawn, IPawnData, IDamageable
    {
        [SerializeField] private PawnData pawnData;
        
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnAttacker pawnAttacker;
        [SerializeField] private PawnMover pawnMover;
        
        [SerializeField] private PawnHealthIndicator pawnHealthIndicator;
        [SerializeField] private PawnPointsIndicator pawnPointsIndicator;
        
        [SerializeField] private GameObject pawnGraphics;
        [SerializeField] private Collider pawnCollider;

        private PawnData _currPawnData;
        public PawnData Data => _currPawnData;
        public PawnPointsIndicator PointsIndicator => pawnPointsIndicator;
        
        public Action onDeath;

        public void Init()
        {
            _currPawnData = Instantiate(pawnData);

            pawnMover.Init(_currPawnData, pawnAnimator, pawnGraphics);
            pawnAttacker.Init(this, pawnAnimator);
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

        public void AttackTarget(IPawn target, Action onAttacked)
        {
            if (pawnAttacker == null)
                Debug.LogError($"{gameObject.name} does not have any attacker component!");
            else
                pawnAttacker.AttackTarget(target, onAttacked);
        }

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
            onDeath?.Invoke();
        }

        #region IPawn Implementation

        public IPawnData PawnData => this;
        public IDamageable Damageable => this;

        public bool IsInteractable()
        {
            return IsAlive();
        }
        
        public bool IsAlive()
        {
            return _currPawnData.Level != 0;
        }
        
        public PawnRelation RelationTo(IPawn pawn)
        {
            return pawn.PawnData.TeamId == _currPawnData.TeamId ? PawnRelation.Friend : PawnRelation.Enemy;
        }

        #endregion
        
        #region IPawnData Implementation

        public Vector3 Position => transform.position;
        public int TeamId => _currPawnData.TeamId;
        public int DamageValue => _currPawnData.Damage;
        public void ModifyLevelBy(int value) { _currPawnData.ModifyLevelBy(value); }

        #endregion
        
        #region IDamageable Implementation

        public void PreDamage(IPawn attacker, Action onPrepared)
        {
            onPrepared += TryBlock;
            RotateTo(attacker.PawnData.Position, onPrepared);
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
    }
}
