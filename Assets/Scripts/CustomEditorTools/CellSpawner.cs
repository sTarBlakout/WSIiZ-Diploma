using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<Cell> allCellsInScene;

        private void Awake()
        {
            allCellsInScene = FindObjectsOfType<Cell>().ToList();
        }
        
        [MenuItem("Window/My Tools/Cell Spawner")]
        public static void ShowWindow() 
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
                
                allCellsInScene = FindObjectsOfType<Cell>().ToList();

                ReasignNeighbours();
            }
            else
            {
                Debug.Log($"Cell already has neighbour at {newCellDirection}");
            }
        }

        private void ReasignNeighbours()
        {
            foreach (var cell in allCellsInScene)
            {
                var collider = cell.GetComponentInChildren<MeshCollider>();
                collider.enabled = false;
                
                foreach (var offset in dirOffset)
                {
                    var offsetPos = new Vector3(
                        cell.transform.position.x + offset.Value.x,
                        cell.transform.position.y,
                        cell.transform.position.z + offset.Value.y);

                    if (Physics.Linecast(cell.transform.position, offsetPos, out var hitInfo))
                    {
                        var collidedCell = hitInfo.collider.transform.parent.GetComponent<Cell>();
                        if (collidedCell != null)
                        {
                            cell.NeighbourCells[(int) offset.Key].cell = collidedCell;
                        }
                    }
                }
                
                collider.enabled = true;
            }
        }
    }
}
