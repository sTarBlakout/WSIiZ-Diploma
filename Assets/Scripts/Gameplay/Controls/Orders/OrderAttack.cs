using Gameplay.Core;

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
}
