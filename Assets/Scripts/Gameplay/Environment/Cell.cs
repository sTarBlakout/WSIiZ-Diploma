using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Environment
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private CellDirection newCellDirection;
        
        private (CellDirection direction, Cell cell)[] neighbourCells =
        {
            (CellDirection.N, null),
            (CellDirection.S, null),
            (CellDirection.NW, null),
            (CellDirection.NE, null),
            (CellDirection.SW, null),
            (CellDirection.SE, null)
        };
        
        public List<Vector3> GetPathToCell()
        {
            return new List<Vector3> { transform.position };
        }

        [ContextMenu("Add Cell")]
        public void AddCell()
        {
            var neighbour = neighbourCells.First(cell => cell.direction == newCellDirection);
            if (neighbour.cell != null) return;
            
            // Add new cell here
        }
    }
}
