using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerMover : MonoBehaviour
    {
        public void MoveToCell(List<Vector3> path)
        {
            Debug.Log($"Player moves to {path[0]}");
        }
    }
}
