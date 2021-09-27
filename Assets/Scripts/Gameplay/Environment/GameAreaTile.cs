using System;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaTile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem reachableTileParticle;
        [SerializeField] private ParticleSystem reachableEnemyParticle;

        public IPawn ContainedPawn { private get; set; }
        public Vector3Int NavPos { get; private set; }
        public Vector3 WorldPos => transform.position;

        public GameAreaTile SetNavPosition(Vector3Int position)
        {
            NavPos = position;
            return this;
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

        private void OnTriggerEnter(Collider other)
        {Debug.Log("here");
            var pawn = other.transform.parent.GetComponent<IPawn>();
            if (pawn == null) return;
            Debug.Log("Enter");
            ContainedPawn = pawn;
        }

        private void OnTriggerExit(Collider other)
        {
            var pawn = other.transform.parent.GetComponent<IPawn>();
            if (pawn == null) return;
            Debug.Log("Exit");
            ContainedPawn = null;
        }
    }
}
