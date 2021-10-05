using Gameplay.Interfaces;
using Gameplay.Pawns;

namespace Gameplay.Controls.Orders
{
    public class OrderArgsInteract : OrderArgsBase
    {
        private IPawnInteractable _target;

        public IPawnInteractable Target => _target;
        
        public OrderArgsInteract(PawnController pawnController, GameArea gameArea) : base(pawnController, gameArea) { }

        public OrderArgsInteract SetInteractable(IPawnInteractable target)
        {
            _target = target;
            return this;
        }
    }
}
