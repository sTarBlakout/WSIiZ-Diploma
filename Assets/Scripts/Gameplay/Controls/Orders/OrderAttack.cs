using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderAttack : OrderBase
    {
        protected OrderArgsAttack args;
        protected CompleteOrderArgsAttack completeArgs;

        public OrderAttack(OrderArgsBase args) : base(args) { this.args = (OrderArgsAttack) args; }

        public override void StartOrder()
        {
            completeArgs = new CompleteOrderArgsAttack();

            if (!args.Target.IsAlive)
            {
                completeArgs.Result = OrderResult.Fail;
                completeArgs.FailReason = OrderFailReason.Dead;
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
            if (args.Target.RelationTo(args.PawnController) != PawnRelation.Enemy)
            {
                completeArgs.Result = OrderResult.Fail;
                completeArgs.FailReason = OrderFailReason.NotAnEnemy;
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
            
            args.PawnController.RotateTo(args.Target.WorldPosition, OnRotated);
        }

        private void OnRotated()
        {
            args.OnUsedActionPointsCallback?.Invoke(1);
            args.PawnController.AttackTarget(args.Target, OnAttacked);
        }

        private void OnAttacked()
        {
            completeArgs.Result = OrderResult.Succes;
            args.OnCompleted?.Invoke(completeArgs);
        }
    }
}
