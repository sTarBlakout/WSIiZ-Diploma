using System;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaTileCollider : MonoBehaviour
    {
        public Action<IPawn> OnPawnEnter;
        public Action<IPawn> OnPawnExit;

        private void OnTriggerEnter(Collider other)
        {
            var pawn = other.GetComponent<IPawn>();
            if (pawn == null) return;
            OnPawnEnter?.Invoke(pawn);
        }

        private void OnTriggerExit(Collider other)
        {
            var pawn = other.GetComponent<IPawn>();
            if (pawn == null) return;
            OnPawnExit?.Invoke(pawn);
        }
    }
}
