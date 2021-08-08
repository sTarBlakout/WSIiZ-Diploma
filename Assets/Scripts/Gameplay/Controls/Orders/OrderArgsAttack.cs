using Gameplay.Interfaces;
using Gameplay.Сharacters;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsAttack : OrderArgsBase
    {
        private IDamageable _damageable;
        private int _maxSteps;

        public IDamageable Damageable => _damageable;
        public int MaxSteps => _maxSteps;

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
    }
}
