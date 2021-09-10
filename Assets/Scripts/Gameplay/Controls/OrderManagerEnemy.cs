using System;
using System.Collections;
using System.Linq;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Controls
{
    public class OrderManagerEnemy : OrderManagerBase
    {
        public override void StartTurn()
        {
            base.StartTurn();
            StartCoroutine(ProcessTurn());
        }

        private IEnumerator ProcessTurn()
        {
            yield return new WaitUntil(() => areAllPathsGenerated);
            _targetPawn ??= GameManager.Instance.PlayerPawn;
            StartOrderAttack(_targetPawn, true);
        }

        protected override bool CanMove()
        {
            return base.CanMove() && CanDoActions();
        }

        protected override void ProcessPostOrder()
        {
            base.ProcessPostOrder();
            if (isTakingTurn && CanDoActions() && CanReachAnyEnemy()) StartOrderAttack(_targetPawn, true);
        }
    }
}
