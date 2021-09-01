using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using SimplePF2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameArea : MonoBehaviour
{
    #region Data

    [Header("General")]
    [SerializeField] private Grid grid;
    [SerializeField] private SimplePathFinding2D pathFinding;
    [SerializeField] private LayerMask tilesLayer;

    [Header("Containers")]
    [SerializeField] private Transform pawnsContainer;
    [SerializeField] private Transform cellsContainer;
    
    [Header("Tilemaps")]
    [SerializeField] private Tilemap permBlockedTilemap;
    [SerializeField] private Tilemap traversableTilemap;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;
    
    public readonly List<PawnController> pawns = new List<PawnController>();
    public readonly List<GameAreaTile> tiles = new List<GameAreaTile>();
    
    private Coroutine _waitPathCor;
    private Path _path;
    private bool _isInit;

    #endregion

    #region Unity Events

    private void Start()
    {
        StartCoroutine(InitGameArea());
    }
    
    #endregion

    #region Initialization

    private IEnumerator InitGameArea()
    {
        yield return new WaitUntil(() => pathFinding.IsInitialised());
        _path = new Path(pathFinding);
        
        pawns.Clear();
        foreach (Transform pawn in pawnsContainer)
        {
            var pawnController = pawn.GetComponent<PawnController>();
            pawnController.Init();

            var cellPosition = grid.WorldToCell(pawn.position);
            pawn.position = grid.GetCellCenterWorld(cellPosition);
            BlockTileAtPos(pawnController.transform.position, true);

            pawns.Add(pawnController);
        }
        
        foreach (Transform cell in cellsContainer)
        {
            var tile = cell.GetComponent<GameAreaTile>();
            tile.SetNavPosition(pathFinding.WorldToNav(cell.position));
            tiles.Add(tile);
        }

        _isInit = true;
    }

    public bool IsInitialized()
    {
        return _isInit;
    }
    
    [ContextMenu("Center Pawns")]
    public void CenterPawns()
    {
        foreach (Transform pawn in pawnsContainer)
        {
            var cellPosition = grid.WorldToCell(pawn.position);
            pawn.position = grid.GetCellCenterWorld(cellPosition);
        }
    }

    [ContextMenu("Initialize Tiles")]
    public void InitializeTiles()
    {
        var destroyList = cellsContainer.Cast<Transform>().ToList();
        foreach (var cell in destroyList) DestroyImmediate(cell.gameObject);
        
        SpawnTilePrefabs(traversableTilemap);
    }

    private void SpawnTilePrefabs(Tilemap tilemap)
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            var sprite = tilemap.GetSprite(pos);
            if (sprite != null)
            {
                var place = tilemap.CellToWorld(pos);
                var cell = (GameObject) PrefabUtility.InstantiatePrefab(cellPrefab);
                cell.transform.position = place;
                cell.transform.parent = cellsContainer;
            }
        }
    }
    
    #endregion

    #region Utilities

    public void BlockTileAtPos(Vector3 worldPos, bool block)
    {
        var navCords = pathFinding.WorldToNav(worldPos);
        var node = pathFinding.GetNode(navCords);
        node.SetBlocked(block);
    }

    public bool IsTileBlocked(Vector3Int navPos)
    {
        return pathFinding.GetNode(navPos).IsBlocked();
    }
    
    public bool IsTileBlocked(Vector3 worldPos)
    {
        var navPos = pathFinding.WorldToNav(worldPos);
        return pathFinding.GetNode(navPos).IsBlocked();
    }
    
    public List<GameAreaTile> GetTilesByDistance(Vector3 fromPos, int distance)
    {
        return Physics.OverlapSphere(fromPos, distance * 2, tilesLayer)
            .Select(coll => coll.transform.parent.GetComponent<GameAreaTile>())
            .ToList();
    }

    #endregion

    #region Generating Paths
    
    public void GeneratePathToPosition(Vector3 fromPos, Vector3 toPos, Action<List<Vector3>> onGeneratedPath)
    {
        var isFromToBlocked = (IsTileBlocked(fromPos), IsTileBlocked(toPos));
        BlockTileAtPos(fromPos, false);
        BlockTileAtPos(toPos, false);
        _path.CreatePath(fromPos, toPos);
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(GeneratePathToPositionCor(fromPos, toPos, isFromToBlocked, onGeneratedPath));
    }

    private IEnumerator GeneratePathToPositionCor(Vector3 fromPos, Vector3 toPos, (bool, bool) isFromToBlocked, Action<List<Vector3>> onGeneratedPath)
    {
        yield return new WaitUntil(() => _path.IsGenerated());
        BlockTileAtPos(fromPos, isFromToBlocked.Item1);
        BlockTileAtPos(toPos, isFromToBlocked.Item2);
        var vectorPath = new List<Vector3>();
        for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
        onGeneratedPath(vectorPath);
    }

    public void GeneratePathsToPawns(PawnController fromPawn, Action<Dictionary<PawnController, List<(Vector3, GameAreaTile)>>> onGeneratedPaths)
    {
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(GeneratePathsToPawnsCor(fromPawn, onGeneratedPaths));
    }
    
    private IEnumerator GeneratePathsToPawnsCor(PawnController fromPawn,  Action<Dictionary<PawnController, List<(Vector3, GameAreaTile)>>> onGeneratedPaths)
    {
        var pathsToPawns = new Dictionary<PawnController, List<(Vector3, GameAreaTile)>>();
        foreach (var pawn in pawns)
        {
            if (fromPawn == pawn) continue;

            var toPos = pawn.transform.position;
            var fromPos = fromPawn.Position;
            var isFromToBlocked = (IsTileBlocked(fromPos), IsTileBlocked(toPos));
            BlockTileAtPos(fromPos, false);
            BlockTileAtPos(toPos, false);
            _path.CreatePath(fromPos, toPos);
            yield return new WaitUntil(() => _path.IsGenerated());
            BlockTileAtPos(fromPos, isFromToBlocked.Item1);
            BlockTileAtPos(toPos, isFromToBlocked.Item2);
            
            var pathPointsList = _path.GetPathPointList();
            var realWorldPath = new List<(Vector3, GameAreaTile)>();
            for (int i = 0; i < pathPointsList.Count; i++) 
                realWorldPath.Add((_path.GetPathPointWorld(i), tiles.First(thisTile => thisTile.NavPosition == pathPointsList[i])));
            pathsToPawns.Add(pawn, realWorldPath);
        }
        onGeneratedPaths?.Invoke(pathsToPawns);
    }

    public void GeneratePathsToReachableTiles(Vector3 fromPos, int distance, Action<Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>>> onGeneratedPaths)
    {
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(GeneratePathToReachableTilesCor(fromPos, distance, GetTilesByDistance(fromPos, distance), onGeneratedPaths));
    }
    
    private IEnumerator GeneratePathToReachableTilesCor(Vector3 fromPos, int distance, List<GameAreaTile> tiles, Action<Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>>> onGeneratedPaths)
    {
        var reachableTiles = new Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>>();
        foreach (var tile in tiles)
        {
            var toPos = tile.transform.position;
            var isFromToBlocked = (IsTileBlocked(fromPos), IsTileBlocked(toPos));
            BlockTileAtPos(fromPos, false);
            BlockTileAtPos(toPos, false);
            _path.CreatePath(fromPos, toPos);
            yield return new WaitUntil(() => _path.IsGenerated());
            BlockTileAtPos(fromPos, isFromToBlocked.Item1);
            BlockTileAtPos(toPos, isFromToBlocked.Item2);
            
            var pathPointsList = _path.GetPathPointList();
            if (pathPointsList.Count - 1 <= distance)
            {
                foreach (var point in pathPointsList)
                {
                    if (tile.NavPosition != point) continue;
                    var realWorldPath = new List<(Vector3, GameAreaTile)>();
                    for (int i = 0; i < pathPointsList.Count; i++) 
                        realWorldPath.Add((_path.GetPathPointWorld(i), this.tiles.First(thisTile => thisTile.NavPosition == pathPointsList[i])));
                    reachableTiles.Add(tile, realWorldPath);
                    break;
                }
            }
        }
        
        onGeneratedPaths?.Invoke(reachableTiles);
    }
    
    #endregion
}
