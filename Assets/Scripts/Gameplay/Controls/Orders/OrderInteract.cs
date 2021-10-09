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
            
            args.PawnController.RotateTo(args.Target.PawnData.Position, OnRotated);
        }
        
        private void OnRotated()
        {
            args.OnUsedActionPointsCallback?.Invoke(1);
            args.Target.PreInteract(args.PawnController, OnPreInteracted);
        }

        private void OnPreInteracted()
        {
            args.Target.Interact(OnInteracted);
        }

        private void OnInteracted()
        {
            args.Target.PostInteract(OnPostInteracted);
        }
        
        private void OnPostInteracted()
        {
            completeArgs.Result = OrderResult.Succes;
            args.OnCompleted?.Invoke(completeArgs);
        }
    }
}
