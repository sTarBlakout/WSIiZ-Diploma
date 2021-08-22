using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Environment;
using Gameplay.Сharacters;
using SimplePF2D;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameArea : MonoBehaviour
{
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

    private void Start()
    {
        StartCoroutine(InitGameArea());
    }

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

    public void GeneratePathToPosition(Vector3 fromPos, Vector3 toPos, Action<List<Vector3>> onGeneratedPath)
    {
        var isFromToBlocked = (IsTileBlocked(fromPos), IsTileBlocked(toPos));
        BlockTileAtPos(fromPos, false);
        BlockTileAtPos(toPos, false);
        _path.CreatePath(fromPos, toPos);
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(WaitGeneratedPath(fromPos, toPos, isFromToBlocked, onGeneratedPath));
    }

    private IEnumerator WaitGeneratedPath(Vector3 fromPos, Vector3 toPos, (bool, bool) isFromToBlocked, Action<List<Vector3>> onGeneratedPath)
    {
        yield return new WaitUntil(() => _path.IsGenerated());
        BlockTileAtPos(fromPos, isFromToBlocked.Item1);
        BlockTileAtPos(toPos, isFromToBlocked.Item2);
        var vectorPath = new List<Vector3>();
        for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
        onGeneratedPath(vectorPath);
    }

    public void GeneratePathsToAllPawns(Vector3 fromPos, Action<Dictionary<PawnController, List<Vector3>>> onGeneratedPaths)
    {
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(GeneratePathsToAllPawnsCor(fromPos, onGeneratedPaths));
    }
    
    private IEnumerator GeneratePathsToAllPawnsCor(Vector3 fromPos, Action<Dictionary<PawnController, List<Vector3>>> onGeneratedPaths)
    {
        var pathsToPawns = new Dictionary<PawnController, List<Vector3>>();
        foreach (var pawn in pawns)
        {
            var toPos = pawn.transform.position;
            var isFromToBlocked = (IsTileBlocked(fromPos), IsTileBlocked(toPos));
            BlockTileAtPos(fromPos, false);
            BlockTileAtPos(toPos, false);
            _path.CreatePath(fromPos, toPos);
            yield return new WaitUntil(() => _path.IsGenerated());
            BlockTileAtPos(fromPos, isFromToBlocked.Item1);
            BlockTileAtPos(toPos, isFromToBlocked.Item2);
            var vectorPath = new List<Vector3>();
            for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
            pathsToPawns.Add(pawn, vectorPath);
        }
        onGeneratedPaths?.Invoke(pathsToPawns);
    }

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

    public List<GameAreaTile> GetReachableTiles(Vector3 fromPos, int distance)
    {
        return Physics.OverlapSphere(fromPos, distance * 2, tilesLayer)
            .Select(coll => coll.transform.parent.GetComponent<GameAreaTile>())
            .ToList();
    }

    public void GetWalkReachableTiles(Vector3 fromPos, int distance, Action<Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>>> onGetReachableTiles)
    {
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(GeneratePathsToTiles(fromPos, distance, GetReachableTiles(fromPos, distance), onGetReachableTiles));
    }
    
    private IEnumerator GeneratePathsToTiles(Vector3 fromPos, int distance, List<GameAreaTile> tiles, Action<Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>>> onGetReachableTiles)
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
        
        onGetReachableTiles?.Invoke(reachableTiles);
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
                var cell = Instantiate(cellPrefab, place, Quaternion.identity);
                cell.transform.parent = cellsContainer;
            }
        }
    }
}
