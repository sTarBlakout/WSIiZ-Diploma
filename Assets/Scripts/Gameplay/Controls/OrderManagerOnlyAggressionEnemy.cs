using System;
using System.Collections;
using System.Linq;
using Gameplay.Core;
using UnityEngine;
using Random = System.Random;

namespace Gameplay.Controls
{
    public class OrderManagerOnlyAggressionEnemy : OrderManagerBase
    {
        private bool _madeRandomMove;
        
        public override void StartTurn()
        {
            base.StartTurn();
            StartCoroutine(ProcessTurn());
        }

        private IEnumerator ProcessTurn()
        {
            yield return new WaitUntil(() => areAllPathsGenerated);
            
            _targetPawnNormal ??= GameManager.Instance.PlayerPawn;
            
            if (!IsWayToPlayerBlocked())
            {
                StartOrderAttack(_targetPawnNormal, true);
            }
            else
            {
                _madeRandomMove = TryRandomMove();
            }
        }

        protected override void ProcessPostOrder()
        {
            if (CanDoActions() && !IsWayToPlayerBlocked() && CanMove())
            {
                StartOrderAttack(_targetPawnNormal, true);
            }
            else
            {
                if (_madeRandomMove)
                {
                    _madeRandomMove = false;
                    CompleteTurn();
                    return;
                }
                
                if (!TryRandomMove())
                {
                    CompleteTurn();
                }
            }
        }

        private bool TryRandomMove()
        {
            if (!CanMove() || pathsToTiles.Count == 0) return false;
            
            UnityEngine.Random.InitState((int) Time.realtimeSinceStartup);
            var randomPath = pathsToTiles.ElementAt(UnityEngine.Random.Range(0, pathsToTiles.Count - 1));
            StartOrderMove(randomPath.Key);
            return true;
        }

        private bool IsWayToPlayerBlocked()
        {
            return pathsToPawns[_targetPawnNormal].Count == 0;
        }
    }
}
