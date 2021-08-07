using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderMove : OrderBase
    {
        protected OrderArgsMove args;
        protected CompleteOrderArgsMove completeArgs;

        public OrderMove(OrderArgsBase args) : base(args) { this.args = (OrderArgsMove) args; }

        public override void StartOrder()
        {
            completeArgs = new CompleteOrderArgsMove();
            
            if (args.GameArea.IsTileBlocked(args.To))
            {
                completeArgs.Result = OrderResult.Fail;
                completeArgs.FailReason = OrderFailReason.BlockedArea;
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
            args.GameArea.GeneratePathToPosition(args.From, args.To, OnPathGenerated);
        }
        
        protected void OnPathGenerated(List<Vector3> path)
        {
            if (path.Count - 1 > args.MaxSteps)
            {
                completeArgs.Result = OrderResult.Fail;
                completeArgs.FailReason = OrderFailReason.TooFar;
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
            
            completeArgs.StepsMoved += path.Count - 1;
            args.GameArea.BlockTileAtPos(path[0], false);
            args.GameArea.BlockTileAtPos(path.Last(), true);
            args.PawnController.MovePath(path, OnReachedDestination);
        }
        
        private void OnReachedDestination()
        {
            completeArgs.Result = OrderResult.Succes;
            args.OnCompleted?.Invoke(completeArgs);
        }
    }
}
