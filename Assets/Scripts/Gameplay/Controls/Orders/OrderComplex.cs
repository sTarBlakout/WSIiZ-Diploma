using System;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderComplex : OrderBase
    {
        protected OrderArgsComplex args;
        protected CompleteOrderArgsComplex completeArgs;
        
        public OrderComplex(OrderArgsBase args) : base(args) { this.args = (OrderArgsComplex) args; }

        public override void StartOrder()
        {
            completeArgs = new CompleteOrderArgsComplex();
            StartNextOrder(null);
        }

        private void StartNextOrder(CompleteOrderArgsBase prevOrderCompleteArgs)
        {
            var nextOrder = args.GetNextOrder();
            if (nextOrder == null)
            {
                args.OnCompleted?.Invoke(completeArgs);
                return;
            }
            nextOrder.AddOnCompleteCallback(StartNextOrder);
            nextOrder.StartOrder();
        }
    }
}
