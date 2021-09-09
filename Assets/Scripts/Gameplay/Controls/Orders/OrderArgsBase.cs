
using System;
using System.Collections.Generic;
using Gameplay.Pawns;
using Gameplay.Environment;
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
        
        private Dictionary<GameAreaTile, List<GameAreaTile>> _pathsToTiles;
        protected Dictionary<PawnController, List<GameAreaTile>> _pathsToPawns;

        public Dictionary<GameAreaTile, List<GameAreaTile>> PathsToTiles => _pathsToTiles;
        public Dictionary<PawnController, List<GameAreaTile>> PathsToPawns => _pathsToPawns;
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
        
        public OrderArgsBase SetPathsToTiles(Dictionary<GameAreaTile, List<GameAreaTile>> pathsToTiles)
        {
            _pathsToTiles = new Dictionary<GameAreaTile, List<GameAreaTile>>(pathsToTiles);
            return this;
        }
        
        public OrderArgsBase SetPathsToPawns(Dictionary<PawnController, List<GameAreaTile>> pathsToPawns)
        {
            _pathsToPawns = new Dictionary<PawnController, List<GameAreaTile>>(pathsToPawns);
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
