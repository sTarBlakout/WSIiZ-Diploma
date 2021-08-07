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
            CompleteTurn();
            // ProcessTurn();
        }

        private void ProcessTurn()
        {
            _damageable ??= GameManager.Instance.PlayerPawn;
            StartOrderAttack(_damageable);
        }
    }
}
