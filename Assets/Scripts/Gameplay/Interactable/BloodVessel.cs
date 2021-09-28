using System;
using System.Collections;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Interactable
{
    public class BloodVessel : MonoBehaviour, IPawn
    {
        [SerializeField] private ParticleSystem bloodEssence;
        
        private void Start()
        {
            StartCoroutine(StartParticleCor());
        }

        private IEnumerator StartParticleCor()
        {
            yield return new WaitForSeconds(2);
            bloodEssence.Play();
        }

        #region IPawn Implementation
        
        public PawnRelation RelationTo(IPawn pawn)
        {
            return PawnRelation.Consumable;
        }

        public bool IsAlive => false;
        public bool IsBlockingTile => false;
        public IPawnData PawnData => null;
        public IDamageable Damageable => null;
        
        #endregion
    }
}
