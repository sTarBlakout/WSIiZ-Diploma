using System;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaTile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem reachableTileParticle;
        
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
                case TileParticleType.Reachable: GameManager.Instance.PlayParticle(reachableTileParticle, activate); break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
