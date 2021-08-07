using System;
using System.Collections.Generic;
using System.Linq;
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
        protected int attacksCurrTurn;
        
        public bool IsTakingTurn => isTakingTurn;
        
        public virtual bool CanTakeTurn()
        {
            return _pawnController.IsAlive();
        }

        public virtual void StartTurn()
        {
            isTakingTurn = true;
        }
        
        protected void CompleteTurn()
        {
            isTakingTurn = false;
            cellsMovedCurrTurn = 0;
            attacksCurrTurn = 0;
        }
        
        #endregion

        #region Order Managment

        protected OrderBase _order;
        protected Order _orderType;
        protected IInteractable _interactable;
        protected IDamageable _damageable;

        protected void StartOrderMove(Vector3 from, Vector3 to)
        {
            var args = new OrderArgsMove(_pawnController, _gameArea);
            args.SetToPos(to)
                .SetFromPos(from)
                .SetMaxSteps(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .AddOnCompleteCallback(OnOrderMoveCompleted);
            
            _order = new OrderMove(args);
            _order.StartOrder();
        }

        protected void OnOrderMoveCompleted(CompleteOrderArgsBase args)
        {
            var moveArgs = (CompleteOrderArgsMove) args;
            if (moveArgs.Result == OrderResult.Succes)
            {
                Debug.Log($"Order status: {moveArgs.Result}    Moved steps: {moveArgs.StepsMoved}");
                cellsMovedCurrTurn += moveArgs.StepsMoved;
                if (_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn == 0) CompleteTurn();
            }
            else
            {
                Debug.Log($"Order status: {moveArgs.Result}    Reason: {moveArgs.FailReason}");
            }
        }

        #region Start Orders

        protected virtual void StartOrderAttack(IDamageable damageable)
        {
            _damageable = damageable;
            if (!_damageable.IsInteractable() || !_damageable.IsEnemyFor(_pawnController)) return;
            _orderType = Order.Attack;
            _gameArea.GeneratePathToPosition(_pawnController.transform.position, _damageable.Position, OnPathGenerated);
        }
        
        #endregion

        #region Make Orders

        private void OrderRotate(Vector3 position)
        {
            _pawnController.RotateTo(position, OnRotated);
        }

        private void OrderMoveForAttacking(List<Vector3> path)
        {
            if (cellsMovedCurrTurn + path.Count - 1 > _pawnController.Data.DistancePerTurn)
            {
                Debug.Log("Enemy is too far!");
                _orderType = Order.None;
                return;
            }
            
            cellsMovedCurrTurn += path.Count - 1;
            path.RemoveAt(path.Count - 1);
            _gameArea.BlockTileAtPos(path[0], false);
            _gameArea.BlockTileAtPos(path.Last(), true);
            _pawnController.MovePath(path, OnReachedDestination);
        }
            
        private void OrderAttack(IDamageable _damageable)
        {
            attacksCurrTurn++;
            _pawnController.AttackTarget(_damageable, OnAttacked);
        }
        
        #endregion
        
        #region Finish Orders

        protected void FinishCurrentOrder()
        {
            switch (_orderType)
            {
                case Order.Attack: FinishOrderAttack(); break;
            }

            _orderType = Order.None;
        }

        protected virtual void FinishOrderAttack()
        {
            if (_pawnController.Data.AttacksPerTurn - attacksCurrTurn == 0) CompleteTurn();
        }
        
        #endregion
        
        #region Callbacks
        
        private void OnRotated()
        {
            switch (_orderType)
            {
                case Order.Attack: OrderAttack(_damageable); break;
                default: FinishCurrentOrder(); break;
            }
        }

        private void OnReachedDestination()
        {
            switch (_orderType)
            {
                case Order.Attack: OrderRotate(_damageable.Position); break;
                case Order.Move: FinishCurrentOrder(); break;
                default: FinishCurrentOrder(); break;
            }
        }

        protected void OnPathGenerated(List<Vector3> path)
        {
            switch (_orderType)
            {
                case Order.Attack: OrderMoveForAttacking(path); break;
                default: FinishCurrentOrder(); break;
            }
        }

        private void OnAttacked()
        {
            Debug.Log("Just Attacked somebody");
            FinishCurrentOrder();
        }
        
        private void OnDeath()
        {
            _gameArea.BlockTileAtPos(transform.position, false);
        }
        
        #endregion
        
        #endregion
    }
}
