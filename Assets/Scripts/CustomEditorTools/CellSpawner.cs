using System.Collections.Generic;
using Gameplay.Environment;
using UnityEditor;
using UnityEngine;

namespace CustomEditorTools
{
    public class CellSpawner : EditorWindow
    {
        private readonly Dictionary<CellDirection, Vector2> dirOffset = new Dictionary<CellDirection, Vector2>()
        {
            { CellDirection.N,  new Vector2(2f , 0f) },
            { CellDirection.S,  new Vector2(-2f , 0f) },
            { CellDirection.NW, new Vector2(1f , 1.6f) },
            { CellDirection.NE, new Vector2(1f , -1.6f) },
            { CellDirection.SW, new Vector2(-1f , 1.6f) },
            { CellDirection.SE, new Vector2(-1f , -1.6f) }
        };
        
        public GameObject cellPrefab;
        public CellDirection newCellDirection;
        public Cell selectedCell;

        [MenuItem("Window/My Tools/Cell Spawner")]
        public static void  ShowWindow() 
        {
            GetWindow(typeof(CellSpawner), false, "Cell Spawner");
        }

        private void OnGUI()
        {
            if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<Cell>() == null)
            {
                EditorGUILayout.LabelField("Select cell in scene !");
            }
            else
            {
                selectedCell = Selection.activeGameObject.GetComponent<Cell>();
                cellPrefab = (GameObject) EditorGUILayout.ObjectField("Cell Prefab", cellPrefab, typeof(GameObject), false);
                newCellDirection = (CellDirection) EditorGUILayout.EnumPopup("Direction", newCellDirection);
            
                if (cellPrefab == null || cellPrefab.GetComponent<Cell>() == null)
                {
                    EditorGUILayout.LabelField("Choose cell prefab !");
                }
                else
                {
                    if (GUILayout.Button($"Add new cell to {newCellDirection}")) CreateCell();
                }
            }
        }

        private void CreateCell()
        {
            if (selectedCell.NeighbourCells[(int)newCellDirection].cell == null)
            {
                Debug.Log($"Added naighbour at {newCellDirection}");
                var newCell = (PrefabUtility.InstantiatePrefab(cellPrefab, selectedCell.transform.parent) as GameObject).GetComponent<Cell>();
                newCell.transform.position = new Vector3(
                    selectedCell.transform.position.x + dirOffset[newCellDirection].x,
                    selectedCell.transform.position.y,
                    selectedCell.transform.position.z + dirOffset[newCellDirection].y);
            }
            else
            {
                Debug.Log($"Cell already has neighbour at {newCellDirection}");
            }
        }
    }
}
