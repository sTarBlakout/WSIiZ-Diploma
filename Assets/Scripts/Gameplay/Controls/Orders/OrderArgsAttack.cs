using Gameplay.Pawns;
using Gameplay.Interfaces;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsAttack : OrderArgsBase
    {
        private IDamageable _damageable;

        public IDamageable Damageable => _damageable;

        public OrderArgsAttack(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea)
        {
        }

        public OrderArgsAttack SetEnemy(IDamageable damageable)
        {
            _damageable = damageable;
            return this;
        }
    }
}
