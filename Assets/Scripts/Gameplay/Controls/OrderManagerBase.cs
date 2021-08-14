using System;
using System.Collections.Generic;
using Gameplay.Controls.Orders;
using Gameplay.Core;
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
        protected Dictionary<PawnController, List<Vector3>> _pathsToPawns;

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
            RefreshPointsIndicator(true);
        }
        
        public void CompleteTurn()
        {
            if (_order != null) return;
            isTakingTurn = false;
            cellsMovedCurrTurn = 0;
            actionsCurrTurn = 0;
            RefreshPointsIndicator(false);
        }
        
        private bool HasMoreActionsToDo()
        {
            return CanMove() || CanDoActions() && CanReachAnyEnemy();
        }
        
        protected bool CanReachAnyEnemy()
        {
            foreach (var pawnPath in _pathsToPawns)
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

        protected void OnOrderMoveCompleted(CompleteOrderArgsBase args)
        {
            var moveArgs = (CompleteOrderArgsMove) args;
            if (moveArgs.Result == OrderResult.Succes)
            {
            }
            else
            {
            }

            OnAnyOrderCompleted();
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

        protected void OnOrderAttackCompleted(CompleteOrderArgsBase args)
        {
            var atkArgs = (CompleteOrderArgsAttack) args;
            if (atkArgs.Result == OrderResult.Succes)
            {
            }
            else
            {
                Debug.Log($"Order status: {atkArgs.Result}    Reason: {atkArgs.FailReason}");
            }

            OnAnyOrderCompleted();
        }

        protected virtual void OnAnyOrderCompleted()
        {
            _order = null;
            _gameArea.GeneratePathsToAllPawns(transform.position, OnPathsToAllPawnsGenerated);
        }

        protected virtual void ProcessPostOrder()
        {
            if (!HasMoreActionsToDo()) CompleteTurn();
        }

        #endregion
        
        #region Callbacks

        private void OnPathsToAllPawnsGenerated(Dictionary<PawnController, List<Vector3>> pathsToPawns)
        {
            _pathsToPawns = pathsToPawns;
            ProcessPostOrder();
        }

        private void OnDeath()
        {
            _gameArea.BlockTileAtPos(transform.position, false);
        }
        
        #endregion
    }
}
