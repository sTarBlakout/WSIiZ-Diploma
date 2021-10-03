using Gameplay.Pawns;
using Gameplay.Interfaces;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsAttack : OrderArgsBase
    {
        private IPawnNormal _target;

        public IPawnNormal Target => _target;

        public OrderArgsAttack(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea)
        {
        }

        public OrderArgsAttack SetEnemy(IPawnNormal target)
        {
            _target = target;
            return this;
        }
    }
}
