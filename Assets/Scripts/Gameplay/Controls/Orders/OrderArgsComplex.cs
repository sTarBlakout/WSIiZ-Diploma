using System.Collections.Generic;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsComplex : OrderArgsBase
    {
        private List<OrderBase> _orders = new List<OrderBase>();
        private int _currentOrderIndex = -1;
        
        public OrderArgsComplex(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea)
        {
        }

        public OrderBase GetNextOrder()
        {
            if (_orders.Count == _currentOrderIndex + 1) return null;
            _currentOrderIndex++;
            return _orders[_currentOrderIndex];
        }

        public OrderArgsComplex AddOrder(OrderBase order)
        {
            _orders.Add(order);
            return this;
        }
    }
}
