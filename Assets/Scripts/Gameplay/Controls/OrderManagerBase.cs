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
            
            _order = null;
        }

        protected virtual void StartOrderAttack(IDamageable damageable)
        {
            var args = new OrderArgsAttack(_pawnController, _gameArea);
            args.SetEnemy(damageable)
                .SetMaxSteps(_pawnController.Data.DistancePerTurn - cellsMovedCurrTurn)
                .AddOnCompleteCallback(OnOrderAttackCompleted);

            _order = new OrderAttack(args);
            _order.StartOrder();
        }

        protected void OnOrderAttackCompleted(CompleteOrderArgsBase args)
        {
            var atkArgs = (CompleteOrderArgsAttack) args;
            if (atkArgs.Result == OrderResult.Succes)
            {
                Debug.Log($"Order status: {atkArgs.Result}    Moved steps: {atkArgs.StepsMoved}");
                attacksCurrTurn++;
                cellsMovedCurrTurn += atkArgs.StepsMoved;
                if (_pawnController.Data.AttacksPerTurn - attacksCurrTurn == 0) CompleteTurn();
            }
            else
            {
                Debug.Log($"Order status: {atkArgs.Result}    Reason: {atkArgs.FailReason}");
            }

            _order = null;
        }

        #endregion
        
        #region Callbacks

        private void OnDeath()
        {
            _gameArea.BlockTileAtPos(transform.position, false);
        }
        
        #endregion
    }
}
