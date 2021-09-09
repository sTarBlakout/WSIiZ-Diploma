using System.Collections.Generic;
using Gameplay.Pawns;
using Gameplay.Environment;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsMove : OrderArgsBase
    {
        private PawnController _toPawn;
        private GameAreaTile _toTile;
        private GameAreaWay _way;
        private Vector3 _to;
        private int _maxSteps;
        private bool _moveAsFarAsCan;

        public PawnController ToPawn => _toPawn;
        public GameAreaTile ToTile => _toTile;
        public GameAreaWay Way => _way;
        public Vector3 To => _to;
        public int MaxSteps => _maxSteps;
        public bool MoveAsFarAsCan => _moveAsFarAsCan;
        
        public OrderArgsMove(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea) { }
        
        public OrderArgsMove SetWay(GameAreaWay way)
        {
            _way = way;
            return this;
        }
        
        public OrderArgsMove SetToPawn(PawnController toPawn)
        {
            _toPawn = toPawn;
            return this;
        }

        public OrderArgsMove SetToTile(GameAreaTile toTile)
        {
            _toTile = toTile;
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
