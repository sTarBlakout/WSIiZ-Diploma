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

        private List<IPawn> _containedPawns = new List<IPawn>();

        public Vector3Int NavPos { get; private set; }
        public Vector3 WorldPos => transform.position;

        public Action<GameAreaTile, bool> OnTileBlocked;

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
            return _containedPawns.Count == 0;
        }

        public IPawn HasEnemyForPawn(IPawn pawn)
        {
            return _containedPawns.First(conPawn => conPawn.RelationTo(pawn) == PawnRelation.Enemy);
        }

        public void ActivateParticle(TileParticleType type, bool activate)
        {
            switch (type)
            {
                case TileParticleType.ReachableTile:
                    if (!reachableEnemyParticle.isPlaying) GameManager.Instance.PlayParticle(reachableTileParticle, activate);
                    break;
                
                case TileParticleType.ReachableEnemy:
                    if (reachableTileParticle.isPlaying) GameManager.Instance.PlayParticle(reachableTileParticle, false);
                    GameManager.Instance.PlayParticle(reachableEnemyParticle, activate);
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void TryBlockTile()
        {
            var block = _containedPawns.Any(pawn => pawn.IsBlockingTile);
            OnTileBlocked?.Invoke(this, block);
        }
    }
}
