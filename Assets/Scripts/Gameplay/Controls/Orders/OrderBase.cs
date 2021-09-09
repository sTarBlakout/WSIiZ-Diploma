
using System;
using UnityEngine.PlayerLoop;

namespace Gameplay.Controls.Orders
{
    public abstract class OrderBase
    {
        private OrderArgsBase _args;

        protected OrderBase(OrderArgsBase args) { _args = args; }
        
        public virtual void Update() { }
        public abstract void StartOrder();

        public void AddOnCompleteCallback(Action<CompleteOrderArgsBase> onCompletedCallback)
        {
            _args.AddOnCompleteCallback(onCompletedCallback);
        }
    }
}
