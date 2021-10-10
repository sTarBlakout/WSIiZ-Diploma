using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderInteract : OrderBase
    {
        protected OrderArgsInteract args;
        protected CompleteOrderArgsInteract completeArgs;
        
        public OrderInteract(OrderArgsBase args) : base(args) { this.args = (OrderArgsInteract) args; }

        public override void StartOrder()
        {
            completeArgs = new CompleteOrderArgsInteract();
            
            args.PawnController.RotateTo(args.Target.WorldPosition, OnRotated);
        }
        
        private void OnRotated()
        {
            args.OnUsedActionPointsCallback?.Invoke(1);
            args.PawnController.InteractWithTarget(args.Target, OnInteracted);
        }

        private void OnInteracted()
        {
            completeArgs.Result = OrderResult.Succes;
            args.OnCompleted?.Invoke(completeArgs);
        }
    }
}
