using Gameplay.Pawns;
using Gameplay.Interfaces;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsAttack : OrderArgsBase
    {
        private IPawn _target;

        public IPawn Target => _target;

        public OrderArgsAttack(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea)
        {
        }

        public OrderArgsAttack SetEnemy(IPawn target)
        {
            _target = target;
            return this;
        }
    }
}
