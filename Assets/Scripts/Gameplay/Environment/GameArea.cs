using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Сharacters;
using SimplePF2D;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private SimplePathFinding2D pathFinding;
    [SerializeField] private Transform pawnsHolder;

    private List<PawnController> _pawns = new List<PawnController>();
    private Coroutine _waitPathCor;
    private Path _path;

    private void Awake()
    {
        _path = new Path(pathFinding);
    }

    private void Start()
    {
        StartCoroutine(InitPawns());
    }

    private IEnumerator InitPawns()
    {
        yield return new WaitUntil(() => pathFinding.IsInitialised());
        
        _pawns.Clear();
        foreach (Transform pawn in pawnsHolder)
        {
            var pawnController = pawn.GetComponent<PawnController>();
            pawnController.Init();

            var cellPosition = grid.WorldToCell(pawn.position);
            pawn.position = grid.GetCellCenterWorld(cellPosition);
            BlockTileAtPos(pawnController.transform.position, true);
            
            _pawns.Add(pawnController);
        }
    }
    
    public void GeneratePathToPosition(Vector3 fromPos, Vector3 toPos, Action<List<Vector3>> onGeneratedPath)
    {
        BlockTileAtPos(fromPos, false);
        BlockTileAtPos(toPos, false);
        _path.CreatePath(fromPos, toPos);
        if (_waitPathCor != null) StopCoroutine(_waitPathCor);
        _waitPathCor = StartCoroutine(WaitGeneratedPath(fromPos, toPos, onGeneratedPath));
    }

    private IEnumerator WaitGeneratedPath(Vector3 fromPos, Vector3 toPos, Action<List<Vector3>> onGeneratedPath)
    {
        yield return new WaitUntil(() => _path.IsGenerated());
        BlockTileAtPos(fromPos, true);
        BlockTileAtPos(toPos, true);
        var vectorPath = new List<Vector3>();
        for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
        onGeneratedPath(vectorPath);
    }

    public void BlockTileAtPos(Vector3 worldPos, bool block)
    {
        var navCords = pathFinding.WorldToNav(worldPos);
        var node = pathFinding.GetNode(navCords);
        node.SetBlocked(block);
    }

    [ContextMenu("Center Pawns")]
    public void CenterPawns()
    {
        foreach (Transform pawn in pawnsHolder)
        {
            var cellPosition = grid.WorldToCell(pawn.position);
            pawn.position = grid.GetCellCenterWorld(cellPosition);
        }
    }
}
