using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Pawns;
using Gameplay.Controls.Orders;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Controls
{
    public abstract class OrderManagerBase : MonoBehaviour
    {
        #region General

        protected PawnController _pawnController;
        protected GameArea _gameArea;

        protected bool areAllPathsGenerated;
        protected Dictionary<IPawn, List<GameAreaTile>> pathsToPawns;
        protected Dictionary<GameAreaTile, List<GameAreaTile>> pathsToTiles;

        protected GameAreaTile currLocationTile;

        public Action<bool> OnTakingTurn;

        protected virtual void Awake()
        {
            _pawnController = GetComponent<PawnController>();
            _gameArea = FindObjectOfType<GameArea>();

            _pawnController.onDeath += OnDeath;
        }

        private void Update()
        {
            _order?.Update();
        }

        #endregion

        #region Turn Managment
        
        protected bool isTakingTurn;
        protected int remainMovePoints;
        protected int remainActionPoints;
        
        public bool IsTakingTurn => isTakingTurn;
        
        public virtual bool CanTakeTurn()
        {
            return _pawnController.IsAlive;
        }

        public virtual void StartTurn()
        {
            isTakingTurn = true;
            remainMovePoints = _pawnController.Data.DistancePerTurn;
            remainActionPoints = _pawnController.Data.ActionsPerTurn;
            GeneratePaths();
            RefreshPointsIndicator(true);
            OnTakingTurn?.Invoke(true);
        }
        
        public virtual void CompleteTurn()
        {
            if (_order != null) return;
            isTakingTurn = false;
            RefreshPointsIndicator(false);
            OnTakingTurn?.Invoke(false);
        }
        
        protected virtual bool HasMoreActionsToDo()
        {
            return CanMove() || CanDoActions() && CanReachAnyEnemy();
        }
        
        protected bool CanReachAnyEnemy()
        {
            foreach (var pawnPath in pathsToPawns)
            {
                if (_pawnController.RelationTo(pawnPath.Key) != PawnRelation.Enemy || !pawnPath.Key.IsAlive) continue;
                if (pawnPath.Value.Count - _pawnController.Data.AttackDistance - 1 <= remainMovePoints) return true;
            }

            return false;
        }

        protected virtual bool CanMove()
        {
            return remainMovePoints != 0;
        }

        protected virtual bool CanDoActions()
        {
            return remainActionPoints != 0;
        }

        protected void UseActionPoints(int value)
        {
            remainActionPoints -= value;
            RefreshPointsIndicator(true);
        }
        
        protected void UseMovePoints(int value)
        {
            remainMovePoints -= value;
            RefreshPointsIndicator(true);
        }

        protected void RefreshPointsIndicator(bool setActive)
        {
            _pawnController.PointsIndicator
                .SetActionPoints(remainActionPoints)
                .SetMovePoints(remainMovePoints)
                .Show(setActive);
        }

        #endregion

        #region Order Managment

        protected OrderBase _order;
        protected GameAreaWay _way;
        protected IPawn _targetPawn;

        protected void StartOrderMove(Vector3 toPos)
        {
            var args = new OrderArgsMove(_pawnController, _gameArea);
            args.SetToPos(toPos)
                .SetMaxSteps(remainMovePoints)
                .AddUsedMovePointsCallback(UseMovePoints)
                .AddOnCompleteCallback(OnOrderMoveCompleted);
            
            _order = new OrderMove(args);
            _order.StartOrder();
        }
        
        protected void StartOrderMove(GameAreaTile toTile)
        {
            var args = new OrderArgsMove(_pawnController, _gameArea);
            args.SetToTile(toTile)
                .SetMaxSteps(remainMovePoints)
                .SetWay(_way)
                .SetPathsToTiles(pathsToTiles)
                .AddUsedMovePointsCallback(UseMovePoints)
                .AddOnCompleteCallback(OnOrderMoveCompleted);

            _order = new OrderMove(args);
            _order.StartOrder();
        }

        protected virtual void StartOrderAttack(IPawn target, bool moveIfTargetFar)
        {
            var argsMove = new OrderArgsMove(_pawnController, _gameArea);
            argsMove.SetToPawn(target)
                .SetMaxSteps(remainMovePoints)
                .SetMoveAsFarAsCan(moveIfTargetFar)
                .SetPathsToPawns(pathsToPawns)
                .AddUsedMovePointsCallback(UseMovePoints);
            var orderMove = new OrderMove(argsMove);

            var argsAttack = new OrderArgsAttack(_pawnController, _gameArea);
            argsAttack.SetEnemy(target).AddUsedActionPointsCallback(UseActionPoints);
            var orderAttack = new OrderAttack(argsAttack);
            
            var argsComplex = new OrderArgsComplex(_pawnController, _gameArea);
            argsComplex.AddOrder(orderMove)
                .AddOrder(orderAttack)
                .AddOnCompleteCallback(OnOrderAttackCompleted);
            _order = new OrderComplex(argsComplex);

            _order.StartOrder();
        }

        protected void OnOrderMoveCompleted(CompleteOrderArgsBase args)
        {
            var moveArgs = (CompleteOrderArgsMove) args;
            currLocationTile = moveArgs.MovedToTile;

            StartCoroutine(OnAnyOrderCompleted());
        }

        protected void OnOrderAttackCompleted(CompleteOrderArgsBase args)
        {
            var complexArgs = (CompleteOrderArgsComplex) args;
            var moveArgs = (CompleteOrderArgsMove) complexArgs.CompleteOrderArgsList.First(completeArgs => completeArgs is CompleteOrderArgsMove);
            currLocationTile = moveArgs.MovedToTile;
            
            StartCoroutine(OnAnyOrderCompleted());
        }

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
            pathsToPawns = null;
            pathsToTiles = null;
            CheckGeneratedPaths();
        }

        protected virtual void CheckGeneratedPaths()
        {
            if (pathsToPawns == null)
            {
                _gameArea.GeneratePathsToPawns(_pawnController, OnPathsToPawnsGenerated);
                return;
            }

            if (pathsToTiles == null)
            {
                _gameArea.GeneratePathsToReachableTiles(transform.position, remainMovePoints, OnPathReachableTilesGenerated);
                return;
            }

            OnAllPathsGenerated();
        }

        #endregion
        
        #region Callbacks

        protected virtual void OnAllPathsGenerated()
        {
            areAllPathsGenerated = true;
        }

        protected virtual void OnPathReachableTilesGenerated(Dictionary<GameAreaTile, List<GameAreaTile>> pathsToTiles)
        {
            this.pathsToTiles = pathsToTiles;
            CheckGeneratedPaths();
        }

        protected virtual void OnPathsToPawnsGenerated(Dictionary<IPawn, List<GameAreaTile>> pathsToPawns)
        {
            this.pathsToPawns = pathsToPawns;
            CheckGeneratedPaths();
        }

        private void OnDeath()
        {
            _gameArea.BlockTileAtPos(transform.position, false);
        }
        
        #endregion

        #region Utilities

        protected bool IsPawnReachable(IPawn pawn)
        {
            return pathsToPawns[pawn].Count - 2 <= remainMovePoints;
        }

        #endregion
    }
}
