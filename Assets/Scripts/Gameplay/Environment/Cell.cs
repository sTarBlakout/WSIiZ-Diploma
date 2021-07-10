using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Environment
{
    public class Cell : MonoBehaviour
    {
        public readonly (CellDirection direction, Cell cell)[] NeighbourCells =
        {
            (CellDirection.N,  null),
            (CellDirection.S,  null),
            (CellDirection.NW, null),
            (CellDirection.NE, null),
            (CellDirection.SW, null),
            (CellDirection.SE, null)
        };
        
        public List<Vector3> GetPathToCell()
        {
            return new List<Vector3> { transform.position };
        }
    }
}
