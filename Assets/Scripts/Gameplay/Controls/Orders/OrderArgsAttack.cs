using Gameplay.Interfaces;
using Gameplay.Сharacters;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsAttack : OrderArgsBase
    {
        private IDamageable _damageable;
        private int _maxSteps;
        private bool _moveIfTargetFar;

        public IDamageable Damageable => _damageable;
        public int MaxSteps => _maxSteps;
        public bool MoveIfTargetFar => _moveIfTargetFar;

        public OrderArgsAttack(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea)
        {
        }

        public OrderArgsAttack SetEnemy(IDamageable damageable)
        {
            _damageable = damageable;
            return this;
        }
        
        public OrderArgsAttack SetMaxSteps(int maxSteps)
        {
            _maxSteps = maxSteps;
            return this;
        }
        
        public OrderArgsAttack SetMoveIfTargetFar(bool moveIfTargetFar)
        {
            _moveIfTargetFar = moveIfTargetFar;
            return this;
        }
    }
}
