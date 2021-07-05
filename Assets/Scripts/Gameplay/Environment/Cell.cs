using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment
{
    public class Cell : MonoBehaviour
    {
        public List<Vector3> GetPathToCell()
        {
            return new List<Vector3> { transform.position };
        }
    }
}
