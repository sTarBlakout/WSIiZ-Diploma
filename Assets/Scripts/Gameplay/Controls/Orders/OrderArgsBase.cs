
using System;
using System.Collections.Generic;
using Gameplay.Environment;
using Gameplay.Сharacters;
using UnityEngine;

namespace Gameplay.Controls.Orders
{
    public abstract class OrderArgsBase
    {
        private PawnController _pawnController;
        private GameArea _gameArea;
        
        private Action<CompleteOrderArgsBase> _onCompletedCallback;
        private Action<int> _onUsedActionPointsCallback;
        private Action<int> _onUsedMovePointsCallback;
        
        private Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>> _pathsToTiles;

        public Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>> PathsToTiles => _pathsToTiles;
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
        
        public OrderArgsBase SetPathsToTiles(Dictionary<GameAreaTile, List<(Vector3, GameAreaTile)>> pathsToTiles)
        {
            _pathsToTiles = pathsToTiles;
            return this;
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
