using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interactable;
using Gameplay.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Pawns
{
    public class PawnController : MonoBehaviour, IPawnNormal
    {
        [Header("Logic Components")]
        [SerializeField] private PawnNormalData pawnData;
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnAttacker pawnAttacker;
        [SerializeField] private PawnDamageable pawnDamageable;
        [SerializeField] private PawnInteractor pawnInteractor;
        [SerializeField] private PawnInventory pawnInventory;
        [SerializeField] private PawnMover pawnMover;
        
        [Header("Indicators")]
        [SerializeField] private PawnHealthIndicator pawnHealthIndicator;
        [SerializeField] private PawnPointsIndicator pawnPointsIndicator;
        
        [Header("Other Components")]
        [SerializeField] private GameObject pawnGraphics;
        [SerializeField] private Collider pawnCollider;

        private PawnNormalData _currPawnData;
        private GameArea _gameArea;
        
        public PawnPointsIndicator PointsIndicator => pawnPointsIndicator;
        
        public Action onDeath;
        private IPawnNormalData _pawnData;

        private void Awake()
        {
            pawnCollider.enabled = false;
        }

        public void Init()
        {
            pawnCollider.enabled = true;
            _currPawnData = Instantiate(pawnData);
            _currPawnData.Init();
            
            _gameArea = FindObjectOfType<GameArea>();

            pawnDamageable.Init(_currPawnData, pawnAnimator, pawnMover, _gameArea, HandleDeath);
            pawnMover.Init(_currPawnData, pawnAnimator, pawnGraphics);
            pawnAttacker.Init(this, pawnAnimator, pawnInventory);
            pawnHealthIndicator.Init(_currPawnData);
            pawnInteractor.Init(this);
            pawnInventory.Init(pawnAnimator, _currPawnData);
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

        public void AttackTarget(IPawnNormal target, Action onAttacked)
        {
            if (pawnAttacker == null)
                Debug.LogError($"{gameObject.name} does not have any attacker component!");
            else
                pawnAttacker.AttackTarget(target, onAttacked);
        }
        
        public void InteractWithTarget(IPawnInteractable target, Action onInteracted)
        {
            if (pawnInteractor == null)
                Debug.LogError($"{gameObject.name} does not have any interactor component!");
            else
                pawnInteractor.InteractWithTarget(target, onInteracted);
        }

        private void HandleDeath()
        {
            StartCoroutine(DeathCoroutine());
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

        public bool IsBlockingTile => IsAlive;
        public bool IsAlive => _currPawnData.BloodLevel != 0;
        public Vector3 WorldPosition => transform.position;

        public IDamageable Damageable => pawnDamageable;
        public IInventory Inventory => pawnInventory;
        public IPawnNormalData PawnData => _currPawnData;

        public Action<GameObject> OnDestroyed { get; set; }
        IPawnData IPawn.PawnData => PawnData;

        public PawnRelation RelationTo(IPawn pawn)
        {
            if (pawn is IPawnNormal normalPawn)
                return normalPawn.PawnData.TeamId == _currPawnData.TeamId
                    ? PawnRelation.Friend
                    : PawnRelation.Enemy;
            
            return PawnRelation.Unknown;
        }

        #endregion
    }
}
