
using System;
using Gameplay.Сharacters;

namespace Gameplay.Controls.Orders
{
    public abstract class OrderArgsBase
    {
        private PawnController _pawnController;
        private GameArea _gameArea;
        private Action<CompleteOrderArgsBase> _onCompletedCallback;

        public PawnController PawnController => _pawnController;
        public GameArea GameArea => _gameArea;
        public Action<CompleteOrderArgsBase> OnCompleted => _onCompletedCallback;
        
        public OrderArgsBase(PawnController pawnController, GameArea gameArea)
        {
            _pawnController = pawnController;
            _gameArea = gameArea;
        }

        public OrderArgsBase AddOnCompleteCallback(Action<CompleteOrderArgsBase> onCompletedCallback)
        {
            _onCompletedCallback += onCompletedCallback;
            return this;
        }
    }
}
