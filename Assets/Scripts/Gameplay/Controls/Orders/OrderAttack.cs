using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls.Orders;
using Gameplay.Core;
using UnityEngine;

public class OrderAttack : OrderBase
{
    protected OrderArgsAttack args;
    protected CompleteOrderArgsAttack completeArgs;
    protected bool isTargetFar = false;

    public OrderAttack(OrderArgsBase args) : base(args) { this.args = (OrderArgsAttack) args; }

    public override void StartOrder()
    {
        completeArgs = new CompleteOrderArgsAttack();
        
        if (!args.Damageable.IsInteractable())
        {
            completeArgs.Result = OrderResult.Fail;
            completeArgs.FailReason = OrderFailReason.NotInteractable;
            args.OnCompleted?.Invoke(completeArgs);
            return;
        }
        if (!args.Damageable.IsEnemyFor(args.PawnController))
        {
            completeArgs.Result = OrderResult.Fail;
            completeArgs.FailReason = OrderFailReason.NotAnEnemy;
            args.OnCompleted?.Invoke(completeArgs);
            return;
        }
        
        args.GameArea.GeneratePathToPosition(args.PawnController.transform.position, args.Damageable.Position, OnPathGenerated);
    }
    
    protected void OnPathGenerated(List<Vector3> path)
    {
        if (path.Count - 1 > args.MaxSteps)
        {
            isTargetFar = true;
            if (args.MoveIfTargetFar)
            {
                path = path.Take(args.MaxSteps + 1).ToList();
            }
            else
            {
                completeArgs.Result = OrderResult.Fail;
                completeArgs.FailReason = OrderFailReason.TooFar;
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
        }
            
        if (!isTargetFar) path.RemoveAt(path.Count - 1);
        completeArgs.StepsMoved += path.Count - 1;
        args.GameArea.BlockTileAtPos(path[0], false);
        args.GameArea.BlockTileAtPos(path.Last(), true);
        args.PawnController.MovePath(path, OnReachedDestination);
    }

    private void OnReachedDestination()
    {
        if (isTargetFar)
        {
            completeArgs.Result = OrderResult.HalfSucces;
            completeArgs.FailReason = OrderFailReason.TooFar;
            args.OnCompleted?.Invoke(completeArgs);
            return;
        }

        args.PawnController.RotateTo(args.Damageable.Position, OnRotated);
    }

    private void OnRotated()
    {
        args.PawnController.AttackTarget(args.Damageable, OnAttacked);
    }

    private void OnAttacked()
    {
        completeArgs.Result = OrderResult.Succes;
        args.OnCompleted?.Invoke(completeArgs);
    }
}
