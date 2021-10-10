using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaTile : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameAreaTileCollider colliderHandler;

        [Header("Native Particles")]
        [SerializeField] private ParticleSystem reachableTileParticle;
        [SerializeField] private ParticleSystem reachableEnemyParticle;
        [SerializeField] private ParticleSystem reachableInteractableParticle;

        private List<IPawn> _containedPawns = new List<IPawn>();

        public Vector3Int NavPos { get; private set; }
        public Vector3 WorldPos => transform.position;

        public Action<GameAreaTile, bool> OnTileBlocked;

        private bool _isBlocked;

        private void Start()
        {
            colliderHandler.OnPawnEnter += Enter;
            colliderHandler.OnPawnExit += Exit;
        }

        public void Enter(IPawn pawn)
        {
            _containedPawns.Add(pawn);
            TryBlockTile();
        }
        
        public void Exit(IPawn pawn)
        {
            _containedPawns.Remove(pawn);
            TryBlockTile();
        }

        public GameAreaTile SetNavPosition(Vector3Int position)
        {
            NavPos = position;
            return this;
        }

        public bool CanWalkIn(IPawn pawn)
        {
            return _containedPawns.All(containedPawn => !containedPawn.IsBlockingTile);
        }

        public IPawnNormal HasEnemyForPawn(IPawn pawn)
        {
            return (IPawnNormal) _containedPawns.FirstOrDefault(conPawn => conPawn.RelationTo(pawn) == PawnRelation.Enemy);
        }
        
        public IPawnInteractable HasInteractableForPawn(IPawn pawn)
        {
            return (IPawnInteractable) _containedPawns.FirstOrDefault(conPawn => conPawn.RelationTo(pawn) == PawnRelation.Interactable);
        }

        public void ActivateParticle(TileParticleType type, bool activate)
        {
            switch (type)
            {
                case TileParticleType.ReachableTile:
                    if (!reachableEnemyParticle.isPlaying && !reachableInteractableParticle.isPlaying) 
                        GameManager.Instance.PlayParticle(reachableTileParticle, activate);
                    break;
                
                case TileParticleType.ReachableEnemy:
                    if (reachableTileParticle.isPlaying) GameManager.Instance.PlayParticle(reachableTileParticle, false);
                    GameManager.Instance.PlayParticle(reachableEnemyParticle, activate);
                    break;
                
                case TileParticleType.ReachableInteractable:
                    if (reachableTileParticle.isPlaying) GameManager.Instance.PlayParticle(reachableTileParticle, false);
                    GameManager.Instance.PlayParticle(reachableInteractableParticle, activate);
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void TryBlockTile()
        {
            var block = _containedPawns.Any(pawn => pawn.IsBlockingTile);
            OnTileBlocked?.Invoke(this, block);
        }
        
        //TODO: Fix when use interactable tile still blocked
    }
}
