using System;
using System.Collections.Generic;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnController : MonoBehaviour, IDamageable
    {
        [SerializeField] private PawnData pawnData;
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnMover pawnMover;
        [SerializeField] private PawnAttacker pawnAttacker;

        private void Start()
        {
            pawnMover.Init(pawnAnimator);
        }

        public void MovePath(List<Vector3> path, Action onReachedDestination)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else

                pawnMover.MovePath(path, onReachedDestination);
        }
        
        public void RotateTo(Vector3 position, Action onRotated)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else
                pawnMover.RotateTo(position, onRotated);
        }

        public void AttackTarget(IDamageable damageable)
        {
            Debug.Log("Pew!!! Attacking :)");
        }

        public Vector3 Position => transform.position;

        public bool IsEnemyFor(PawnController pawn)
        {
            return pawnData.TeamId != pawn.pawnData.TeamId;
        }
    }
}
