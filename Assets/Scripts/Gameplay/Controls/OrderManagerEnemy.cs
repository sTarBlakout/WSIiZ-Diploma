using System;
using System.Linq;
using Gameplay.Core;
using Gameplay.Сharacters;

namespace Gameplay.Controls
{
    public class OrderManagerEnemy : OrderManagerBase
    {
        public override void StartTurn()
        {
            base.StartTurn();
            ProcessTurn();
        }

        private void ProcessTurn()
        {
            _damageable ??= GameManager.Instance.PlayerPawn;
            
            _order = Order.Attack;
            _gameArea.GeneratePathToPosition(_pawnController.transform.position, _damageable.Position, OnPathGenerated);
        }
    }
}
