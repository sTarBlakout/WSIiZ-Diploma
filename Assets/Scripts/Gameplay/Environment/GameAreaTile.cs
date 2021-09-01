using System;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaTile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem reachableTileParticle;
        [SerializeField] private ParticleSystem reachableEnemyParticle;
        
        public Vector3Int NavPosition { get; private set; }

        public GameAreaTile SetNavPosition(Vector3Int position)
        {
            NavPosition = position;
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
    }
}
