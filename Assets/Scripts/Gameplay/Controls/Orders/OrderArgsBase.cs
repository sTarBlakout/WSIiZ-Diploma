
using System;
using Gameplay.Сharacters;

namespace Gameplay.Controls.Orders
{
    public abstract class OrderArgsBase
    {
        private PawnController _pawnController;
        private GameArea _gameArea;
        
        private Action<CompleteOrderArgsBase> _onCompletedCallback;
        private Action<int> _onUsedActionPointsCallback;
        private Action<int> _onUsedMovePointsCallback;

        public PawnController PawnController => _pawnController;
        public GameArea GameArea => _gameArea;
        public Action<CompleteOrderArgsBase> OnCompleted => _onCompletedCallback;
        public Action<int> OnUsedActionPointsCallback => _onUsedActionPointsCallback;
        public Action<int> OnUsedMovePointsCallback => _onUsedMovePointsCallback;
        
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
        
        public OrderArgsBase AddUsedActionPointsCallback(Action<int> onUsedActionPointsCallback)
        {
            _onUsedActionPointsCallback = onUsedActionPointsCallback;
            return this;
        }
        
        public OrderArgsBase AddUsedMovePointsCallback(Action<int> onUsedMovePointsCallback)
        {
            _onUsedMovePointsCallback = onUsedMovePointsCallback;
            return this;
        }
    }
}
