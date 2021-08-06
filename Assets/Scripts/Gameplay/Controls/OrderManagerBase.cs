using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls
{
    public abstract class OrderManagerBase : MonoBehaviour
    {
        protected bool isTakingTurn;
        protected int cellsMovedCurrTurn;
        protected int attacksCurrTurn;
        
        protected PawnController _pawnController;
        protected GameArea _gameArea;

        protected Order _order;
        protected IInteractable _interactable;
        protected IDamageable _damageable;
        
        public bool IsTakingTurn => isTakingTurn;
    
        protected virtual void Awake()
        {
            _pawnController = GetComponent<PawnController>();
            _gameArea = FindObjectOfType<GameArea>();

            _pawnController.onDeath += OnDeath;
        }

        #region Turn Managment
        
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

        #region Start Orders

        protected virtual void StartOrderMove(Vector3 fromPos, Vector3 toPos)
        {
            if (_gameArea.IsTileBlocked(toPos))
            {
                Debug.Log("Tile is blocked!");
                return;
            }
            _order = Order.Move;
            _gameArea.GeneratePathToPosition(fromPos, toPos, OnPathGenerated);
        }

        protected virtual void StartOrderAttack(IDamageable damageable)
        {
            _damageable = damageable;
            if (!_damageable.IsInteractable() || !_damageable.IsEnemyFor(_pawnController)) return;
            _order = Order.Attack;
            _gameArea.GeneratePathToPosition(_pawnController.transform.position, _damageable.Position, OnPathGenerated);
        }
        
        #endregion
        
        #region Finish Orders

        protected void FinishCurrentOrder()
        {
            switch (_order)
            {
                case Order.Attack: FinishOrderAttack(); break;
                case Order.Move: FinishOrderMove(); break;
            }

            _order = Order.None;
        }

        protected virtual void FinishOrderMove()
        {
            if (_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn == 0) CompleteTurn();
        }
        
        protected virtual void FinishOrderAttack()
        {
            if (_pawnController.Data.AttacksPerTurn - attacksCurrTurn == 0) CompleteTurn();
        }
        
        #endregion

        #region Orders

        private void OrderRotate(Vector3 position)
        {
            _pawnController.RotateTo(position, OnRotated);
        }
    
        private void OrderSimpleMove(List<Vector3> path)
        {
            if (cellsMovedCurrTurn + path.Count - 1 > _pawnController.Data.DistancePerTurn)
            {
                Debug.Log("Target point is too far!");
                _order = Order.None;
                return;
            }
            
            cellsMovedCurrTurn += path.Count - 1;
            _gameArea.BlockTileAtPos(path[0], false);
            _gameArea.BlockTileAtPos(path.Last(), true);
            _pawnController.MovePath(path, OnReachedDestination);
        }
            
        private void OrderMoveForAttacking(List<Vector3> path)
        {
            if (cellsMovedCurrTurn + path.Count - 1 > _pawnController.Data.DistancePerTurn)
            {
                Debug.Log("Enemy is too far!");
                _order = Order.None;
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
        
        #region Callbacks
        
        private void OnRotated()
        {
            switch (_order)
            {
                case Order.Attack: OrderAttack(_damageable); break;
                default: FinishCurrentOrder(); break;
            }
        }

        private void OnReachedDestination()
        {
            switch (_order)
            {
                case Order.Attack: OrderRotate(_damageable.Position); break;
                case Order.Move: FinishCurrentOrder(); break;
                default: FinishCurrentOrder(); break;
            }
        }

        protected void OnPathGenerated(List<Vector3> path)
        {
            switch (_order)
            {
                case Order.Move: OrderSimpleMove(path); break;
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
    }
}
