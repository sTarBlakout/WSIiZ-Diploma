using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsMove : OrderArgsBase
    {
        private Vector3 _from;
        private Vector3 _to;
        private int _maxSteps;

        public Vector3 From => _from;
        public Vector3 To => _to;
        public int MaxSteps => _maxSteps;
        
        
        public OrderArgsMove(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea) { }

        public OrderArgsMove SetFromPos(Vector3 from)
        {
            _from = from;
            return this;
        }
        
        public OrderArgsMove SetToPos(Vector3 to)
        {
            _to = to;
            return this;
        }

        public OrderArgsMove SetMaxSteps(int maxSteps)
        {
            _maxSteps = maxSteps;
            return this;
        }
    }
}
