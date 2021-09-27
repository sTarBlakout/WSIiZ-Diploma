using System;
using System.Collections.Generic;
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

        private void OnEnable()
        {
            colliderHandler.OnPawnEnter += PawnEntered;
            colliderHandler.OnPawnExit += PawnExited;
        }

        private void OnDisable()
        {
            colliderHandler.OnPawnEnter -= PawnEntered;
            colliderHandler.OnPawnExit -= PawnExited;
        }

        private void PawnEntered(IPawn pawn)
        {
            _containedPawns.Add(pawn);
        }
        
        private void PawnExited(IPawn pawn)
        {
            _containedPawns.Remove(pawn);
        }

        public void SetNavPosition(Vector3Int position)
        {
            NavPos = position;
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
    }
}
