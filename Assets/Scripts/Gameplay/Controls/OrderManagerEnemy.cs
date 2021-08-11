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
            StartOrderAttack(_damageable, true);
        }

        protected override bool CanMove()
        {
            return base.CanMove() && CanDoActions();
        }

        protected override void ProcessPostOrder()
        {
            base.ProcessPostOrder();
            if (isTakingTurn && CanDoActions() && CanReachAnyEnemy()) StartOrderAttack(_damageable, true);
        }
    }
}
