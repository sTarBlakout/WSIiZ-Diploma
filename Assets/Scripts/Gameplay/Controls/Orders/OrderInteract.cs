using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderInteract : OrderBase
    {
        protected OrderArgsInteract args;
        
        public OrderInteract(OrderArgsBase args) : base(args) { this.args = (OrderArgsInteract) args; }

        public override void StartOrder()
        {
            Debug.Log("INTERACT");
        }
    }
}
