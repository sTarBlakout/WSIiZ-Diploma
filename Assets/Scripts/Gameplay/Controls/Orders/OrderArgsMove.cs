using System.Collections.Generic;
using Gameplay.Environment;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsMove : OrderArgsBase
    {
        private GameAreaTile _toTile;
        private Vector3 _from;
        private Vector3 _to;
        private int _maxSteps;
        private bool _moveAsFarAsCan;

        public GameAreaTile ToTile => _toTile;
        public Vector3 From => _from;
        public Vector3 To => _to;
        public int MaxSteps => _maxSteps;
        public bool MoveAsFarAsCan => _moveAsFarAsCan;


        public OrderArgsMove(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea) { }

        public OrderArgsMove SetToTile(GameAreaTile toTile)
        {
            _toTile = toTile;
            return this;
        }
        
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
        
        public OrderArgsMove SetMoveAsFarAsCan(bool value)
        {
            _moveAsFarAsCan = value;
            return this;
        }
    }
}
