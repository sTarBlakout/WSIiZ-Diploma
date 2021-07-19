using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnController : MonoBehaviour
    {
        [SerializeField] private PawnData pawnData;
        [SerializeField] private PawnMover pawnMover;

        public void MovePath(List<Vector3> path, Action onReachedDestination)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else
                pawnMover.MovePath(path);
        }

        public bool IsEnemyFor(PawnController pawn)
        {
            return pawnData.TeamId != pawn.pawnData.TeamId;
        }
    }
}
