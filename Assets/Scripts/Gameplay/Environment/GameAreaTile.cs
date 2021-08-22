using UnityEngine;
using UnityEngine.WSA;

namespace Gameplay.Environment
{
    public class GameAreaTile : MonoBehaviour
    {
        public Vector3Int NavPosition { get; private set; }

        public GameAreaTile SetNavPosition(Vector3Int position)
        {
            NavPosition = position;
            return this;
        }
    }
}
