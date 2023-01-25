using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using Global;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class OrderManagerPlayer : OrderManagerBase
    {
        [SerializeField] private AudioClip cellClickedSound;
        
        public IPawn Player => _pawnController;
        [HideInInspector] public GameAreaTile selectedTile;

        public Action<GameAreaTile> OnTileClicked;
        public Action<IPawn> OnPawnClicked;

        public bool inputBlocked;

        #region Finger Handling

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
            if (_order != null || !isTakingTurn || !areAllPathsGenerated || selectedTile != null || inputBlocked) return;
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;

            var tile = hitInfo.collider.transform.parent.GetComponent<GameAreaTile>();
            if (tile != null)
            {
                AudioManager.Instance.PlaySound(cellClickedSound);
                
                // Clicked on empty tile, proceed with movement
                if (tile.CanWalkIn(_pawnController) && pathsToTiles.ContainsKey(tile))
                {
                    selectedTile = tile;
                    DrawWay(true, OrderType.Move);
                    OnTileClicked?.Invoke(tile);
                    return;
                }

                // Clicked on tile with enemy pawn, proceed with attack
                _targetPawnNormal = tile.HasEnemyForPawn(_pawnController);
                if (_targetPawnNormal != null && IsPawnReachable(_targetPawnNormal) && _targetPawnNormal.IsAlive)
                {
                    var pathToPawn = pathsToPawns[_targetPawnNormal];
                    selectedTile = pathToPawn[pathToPawn.Count - 2];
                    DrawWay(true, OrderType.Attack);
                    OnPawnClicked?.Invoke(_targetPawnNormal);
                    return;
                }

                // Clicked on tile with interactable pawn, proceed with interaction
                _targetPawnInteractable = tile.HasInteractableForPawn(_pawnController);
                if (_targetPawnInteractable != null && IsPawnReachable(_targetPawnInteractable))
                {
                    var pathToPawn = pathsToPawns[_targetPawnInteractable];
                    selectedTile = pathToPawn[pathToPawn.Count - 2];
                    DrawWay(true, OrderType.Interact);
                    OnPawnClicked?.Invoke(_targetPawnInteractable);
                    return;
                }
            }
        }
        
        #endregion

        #region OrderManagment

        public void StartOrder(OrderType order)
        {
            HighlightReachableTiles(false);
            HighlightEnemyTiles(false);
            HighlightInteractableTiles(false);
            
            switch (order)
            {
                case OrderType.Interact: StartOrderInteract(_targetPawnInteractable, false); break;
                case OrderType.Attack: StartOrderAttack(_targetPawnNormal, false); break;
                case OrderType.Move: StartOrderMove(selectedTile); break;
            }
        }
        

        public void ResetOrder()
        {
            DrawWay(false);
            selectedTile = null;
            _targetPawnNormal = null;
            _targetPawnInteractable = null;
        }
        
        #endregion

        #region InventoryManagment

        public bool HasAnyItems(ItemType type)
        {
            return _pawnController.Inventory.HasAnyItems(type);
        }

        public List<(IItem, bool)> GetItems(ItemType type)
        {
            return _pawnController.Inventory.GetInventoryItems(type);
        }

        public void EquipItem(IItem item)
        {
            _pawnController.Inventory.EquipItem(item);
        }

        #endregion

        #region Visuals Managment
        
        private void DrawWay(bool draw, OrderType order = OrderType.None)
        {
            if (draw)
            {
                if (order == OrderType.None) return;

                _way = _gameArea.CreateWay().SetFollowPawn(_pawnController.transform).SetOrder(order);

                if (selectedTile != currLocationTile)
                {
                    var optimizedPath = _gameArea.OptimizePathForPawn(pathsToTiles[selectedTile], _pawnController.transform, order);
                    
                    _way.SetWayLine(_pawnController.PawnData.WayMoveLinePrefab)
                        .BuildWay(optimizedPath);
                }

                if (order == OrderType.Attack)
                {
                    _way.SetAttackLine(_pawnController.PawnData.WayAttackLinePrefab)
                        .BuildWayToPawn(_targetPawnNormal);
                }

                if (order == OrderType.Interact)
                {
                    _way.SetInteractableLine(_pawnController.PawnData.WayInteractLinePrefab)
                        .BuildWayToPawn(_targetPawnInteractable);
                }
            }
            else
            {
                if (_way == null) return;
                _way.DestroyWay();
                _way = null;
            }
        }

        private void HighlightReachableTiles(bool highlight)
        {
            var tilesList = new List<GameAreaTile>(pathsToTiles.Keys);
            foreach (var tile in tilesList)
            {
                if (_gameArea.IsTileBlocked(tile.NavPos)) continue;
                tile.ActivateParticle(TileParticleType.ReachableTile, highlight);
            }
        }
        
        private void HighlightEnemyTiles(bool highlight)
        {
            var pathsToNormalPawns = pathsToPawns.Where(pawnPath => pawnPath.Key is IPawnNormal)
                .ToDictionary(pawnPath => pawnPath.Key as IPawnNormal, pawnPath => pawnPath.Value);

            var pathsToEnemies = pathsToNormalPawns.Where(pawnPath =>
                pawnPath.Key.RelationTo(_pawnController) == PawnRelation.Enemy
                && IsPawnReachable(pawnPath.Key)
                && pawnPath.Key.IsAlive).ToList();

            pathsToEnemies.RemoveAll(pawnPath => pawnPath.Value.Count == 0);

            var tilesList = pathsToEnemies.Select(pathToEnemy => pathToEnemy.Value[pathToEnemy.Value.Count - 1]).ToList();
            foreach (var tile in tilesList) tile.ActivateParticle(TileParticleType.ReachableEnemy, highlight);
        }

        private void HighlightInteractableTiles(bool highlight)
        {
            var pathsToInteractables = pathsToPawns.Where(pawnPath =>
                    pawnPath.Key.RelationTo(_pawnController) == PawnRelation.Interactable &&
                    IsPawnReachable(pawnPath.Key)).ToList();
            
            pathsToInteractables.RemoveAll(pawnPath => pawnPath.Value.Count == 0);

            var tilesList = pathsToInteractables.Select(pathsToInteractable => pathsToInteractable.Value[pathsToInteractable.Value.Count - 1]).ToList();
            foreach (var tile in tilesList) tile.ActivateParticle(TileParticleType.ReachableInteractable, highlight);
        }
        
        #endregion

        #region Overrides

        protected override void OnAllPathsGenerated()
        {
            base.OnAllPathsGenerated();
            HighlightReachableTiles(true);
            HighlightEnemyTiles(true);
            HighlightInteractableTiles(true);
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
            HighlightInteractableTiles(false);
            ResetOrder();
            base.CompleteTurn();
        }

        protected override bool CanDoActions() { return true; }

        protected override bool CanMove() { return true; }
        
        #endregion
    }
}
