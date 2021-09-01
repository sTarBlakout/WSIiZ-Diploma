using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class OrderManagerPlayer : OrderManagerBase
    {
        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }
        
        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (_order != null || !isTakingTurn || !areAllPathsGenerated) return;
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;

            // Clicked on map, process simple movement
            var tile = hitInfo.collider.transform.parent.GetComponent<GameAreaTile>();
            if (tile != null)
            {
                HighlightReachableTiles(false);
                HighlightEnemyTiles(false);
                StartOrderMove(tile);
                return;
            }
            
            // Checking if interactable
            _interactable = hitInfo.collider.transform.parent.GetComponent<IInteractable>();
            
            if (_interactable is IDamageable damageable)
            {
                HighlightReachableTiles(false);
                HighlightEnemyTiles(false);
                StartOrderAttack(damageable, false);
                return;
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
                pawnPath.Key.IsEnemyFor(_pawnController) && pawnPath.Value.Count - 2 <= _pawnController.Data.DistancePerTurn - cellsMovedCurrTurn && pawnPath.Key.IsAlive());
            var tilesList = pathsToEnemies.Select(pathToEnemy => pathToEnemy.Value[pathToEnemy.Value.Count - 1].Item2).ToList();
            foreach (var tile in tilesList) tile.ActivateParticle(TileParticleType.ReachableEnemy, highlight);
        }

        protected override void OnAllPathsGenerated()
        {
            base.OnAllPathsGenerated();
            HighlightReachableTiles(true);
            HighlightEnemyTiles(true);
        }

        protected override bool CanDoActions() { return true; }

        protected override bool CanMove() { return true; }
    }
}
