using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class OrderManagerPlayer : OrderManagerBase
    {
        public GameAreaTile selectedTile;
        
        public Action<GameAreaTile> OnTileClicked;

        #region Finger Handling

        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
            
            LeanTouch.OnFingerDown += HandleFingerDown;
            LeanTouch.OnFingerUpdate += HandleFingerUpdate;
            LeanTouch.OnFingerUp += HandleFingerUp;
        }
        
        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
            
            LeanTouch.OnFingerDown -= HandleFingerDown;
            LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
            LeanTouch.OnFingerUp -= HandleFingerUp;
        }

        private void HandleFingerDown(LeanFinger finger)
        {
            
        }
        
        private void HandleFingerUpdate(LeanFinger finger)
        {
            
        }

        private void HandleFingerUp(LeanFinger finger)
        {
            
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (_order != null || !isTakingTurn || !areAllPathsGenerated || selectedTile != null) return;
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;

            // Clicked on map, process simple movement
            var tile = hitInfo.collider.transform.parent.GetComponent<GameAreaTile>();
            if (tile != null)
            {
                if (pathsToTiles.ContainsKey(tile))
                {
                    selectedTile = tile;
                    DrawWay(true);
                    OnTileClicked?.Invoke(tile);
                }
                return;
            }
            
            // Checking if interactable
            _targetPawn = hitInfo.collider.transform.parent.GetComponent<IPawn>();
            if (_targetPawn != null)
            {
                if (_targetPawn.Damageable != null)
                {
                    HighlightReachableTiles(false);
                    HighlightEnemyTiles(false);
                    StartOrderAttack(_targetPawn, false);
                    return;
                }
            }
        }
        
        #endregion

        public void StartOrder(OrderType order)
        {
            HighlightReachableTiles(false);
            HighlightEnemyTiles(false);
            
            switch (order)
            {
                case OrderType.Attack: break;
                case OrderType.Move: StartOrderMove(selectedTile); break;
            }
        }

        public void ResetOrder()
        {
            DrawWay(false);
            selectedTile = null;
        }

        private void DrawWay(bool draw)
        {
            if (draw)
            {
                _way = _gameArea.CreateWay();
                _way.SetWayLine(_pawnController.Data.WayMoveLinePrefab)
                    .BuildWay(_gameArea.OptimizePathForPawn(pathsToTiles[selectedTile], _pawnController.transform))
                    .SetFollowPawn(_pawnController.transform);
            }
            else
            {
                if (_way == null) return;
                _way.DestroyWay();
            }
        }

        private void HighlightReachableTiles(bool highlight)
        {
            var tilesList = new List<GameAreaTile>(pathsToTiles.Keys);
            foreach (var tile in tilesList) tile.ActivateParticle(TileParticleType.ReachableTile, highlight);
        }
        
        private void HighlightEnemyTiles(bool highlight)
        {
            var pathsToEnemies = pathsToPawns.Where(pawnPath => 
                pawnPath.Key.RelationTo(_pawnController) == PawnRelation.Enemy 
                && pawnPath.Value.Count - 2 <= _pawnController.Data.DistancePerTurn - cellsMovedCurrTurn 
                && pawnPath.Key.IsAlive());
            var tilesList = pathsToEnemies.Select(pathToEnemy => pathToEnemy.Value[pathToEnemy.Value.Count - 1]).ToList();
            foreach (var tile in tilesList) tile.ActivateParticle(TileParticleType.ReachableEnemy, highlight);
        }

        protected override void OnAllPathsGenerated()
        {
            base.OnAllPathsGenerated();
            HighlightReachableTiles(true);
            HighlightEnemyTiles(true);
        }

        protected override void ProcessPostOrder()
        {
            base.ProcessPostOrder();
            ResetOrder();
        }

        public override void CompleteTurn()
        {
            HighlightReachableTiles(false);
            HighlightEnemyTiles(false);
            base.CompleteTurn();
        }

        protected override bool CanDoActions() { return true; }

        protected override bool CanMove() { return true; }
    }
}
