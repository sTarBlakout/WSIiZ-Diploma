using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls.Orders;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls
{
    public abstract class OrderManagerBase : MonoBehaviour
    {
        #region General

        protected PawnController _pawnController;
        protected GameArea _gameArea;

        protected bool areAllPathsGenerated;
        protected Dictionary<PawnController, List<(Vector3, GameAreaTile)>> pathsToEnemies;
        protected Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>> pathsToTiles;

        public Action<bool> OnTakingTurn;

        protected virtual void Awake()
        {
            _pawnController = GetComponent<PawnController>();
            _gameArea = FindObjectOfType<GameArea>();

            _pawnController.onDeath += OnDeath;
        }

        #endregion

        #region Turn Managment
        
        protected bool isTakingTurn;
        protected int cellsMovedCurrTurn;
        protected int actionsCurrTurn;
        
        public bool IsTakingTurn => isTakingTurn;
        
        public virtual bool CanTakeTurn()
        {
            return _pawnController.IsAlive();
        }

        public virtual void StartTurn()
        {
            isTakingTurn = true;
            GeneratePaths();
            RefreshPointsIndicator(true);
            OnTakingTurn?.Invoke(true);
        }
        
        public void CompleteTurn()
        {
            if (_order != null) return;
            isTakingTurn = false;
            cellsMovedCurrTurn = 0;
            actionsCurrTurn = 0;
            RefreshPointsIndicator(false);
            OnTakingTurn?.Invoke(false);
        }
        
        protected virtual bool HasMoreActionsToDo()
        {
            return CanMove() || CanDoActions() && CanReachAnyEnemy();
        }
        
        protected bool CanReachAnyEnemy()
        {
            foreach (var pawnPath in pathsToEnemies)
            {
                if (!_pawnController.IsEnemyFor(pawnPath.Key) || !pawnPath.Key.IsAlive()) continue;
                if (pawnPath.Value.Count - _pawnController.Data.AttackDistance - 1 <= _pawnController.Data.DistancePerTurn - cellsMovedCurrTurn) return true;
            }

            return false;
        }

        protected virtual bool CanMove()
        {
            return _pawnController.Data.DistancePerTurn - cellsMovedCurrTurn != 0;
        }

        protected virtual bool CanDoActions()
        {
            return _pawnController.Data.ActionsPerTurn - actionsCurrTurn != 0;
        }

        protected void UseActionPoints(int value)
        {
            actionsCurrTurn += value;
            RefreshPointsIndicator(true);
        }
        
        protected void UseMovePoints(int value)
        {
            cellsMovedCurrTurn += value;
            RefreshPointsIndicator(true);
        }

        protected void RefreshPointsIndicator(bool setActive)
        {
            _pawnController.PointsIndicator
                .SetActionPoints(_pawnController.Data.ActionsPerTurn - actionsCurrTurn)
                .SetMovePoints(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .Show(setActive);
        }

        #endregion

        #region Order Managment

        protected OrderBase _order;
        protected IInteractable _interactable;
        protected IDamageable _damageable;

        protected void StartOrderMove(Vector3 from, Vector3 to)
        {
            var args = new OrderArgsMove(_pawnController, _gameArea);
            args.SetToPos(to)
                .SetFromPos(from)
                .SetMaxSteps(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .AddUsedMovePointsCallback(UseMovePoints)
                .AddOnCompleteCallback(OnOrderMoveCompleted);
            
            _order = new OrderMove(args);
            _order.StartOrder();
        }
        
        protected void StartOrderMove(GameAreaTile toTile)
        {
            var args = new OrderArgsMove(_pawnController, _gameArea);
            args.SetToTile(toTile)
                .SetMaxSteps(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .AddUsedMovePointsCallback(UseMovePoints)
                .AddOnCompleteCallback(OnOrderMoveCompleted)
                .SetPathsToTiles(pathsToTiles);
            
            _order = new OrderMove(args);
            _order.StartOrder();
        }

        protected virtual void StartOrderAttack(IDamageable damageable, bool moveIfTargetFar)
        {
            var args = new OrderArgsAttack(_pawnController, _gameArea);
            args.SetEnemy(damageable)
                .SetMaxSteps(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .SetMoveIfTargetFar(moveIfTargetFar)
                .AddOnCompleteCallback(OnOrderAttackCompleted)
                .AddUsedMovePointsCallback(UseMovePoints)
                .AddUsedActionPointsCallback(UseActionPoints);

            _order = new OrderAttack(args);
            _order.StartOrder();
        }

        protected void OnOrderMoveCompleted(CompleteOrderArgsBase args) { StartCoroutine(OnAnyOrderCompleted()); }
        
        protected void OnOrderAttackCompleted(CompleteOrderArgsBase args) { StartCoroutine(OnAnyOrderCompleted()); }

        protected virtual IEnumerator OnAnyOrderCompleted()
        {
            _order = null;
            GeneratePaths();
            yield return new WaitUntil(() => areAllPathsGenerated);
            ProcessPostOrder();
        }

        protected virtual void ProcessPostOrder()
        {
            if (!HasMoreActionsToDo()) CompleteTurn();
        }

        protected virtual void GeneratePaths()
        {
            areAllPathsGenerated = false;
            pathsToEnemies = null;
            pathsToTiles = null;
            CheckGeneratedPaths();
        }

        protected virtual void CheckGeneratedPaths()
        {
            if (pathsToEnemies == null)
            {
                _gameArea.GeneratePathsToAllEnemies(_pawnController, OnPathsToEnemiesGenerated);
                return;
            }

            if (pathsToTiles == null)
            {
                _gameArea.GeneratePathsToReachableTiles(transform.position, _pawnController.Data.DistancePerTurn - cellsMovedCurrTurn, OnPathReachableTilesGenerated);
                return;
            }

            areAllPathsGenerated = true;
        }

        #endregion
        
        #region Callbacks

        protected virtual void OnPathReachableTilesGenerated(Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>> pathsToTiles)
        {
            this.pathsToTiles = pathsToTiles;
            CheckGeneratedPaths();
        }

        private void OnPathsToEnemiesGenerated(Dictionary<PawnController, List<(Vector3, GameAreaTile)>> pathsToEnemies)
        {
            this.pathsToEnemies = pathsToEnemies;
            CheckGeneratedPaths();
        }

        private void OnDeath()
        {
            _gameArea.BlockTileAtPos(transform.position, false);
        }
        
        #endregion
    }
}
