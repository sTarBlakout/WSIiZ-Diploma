using System;
using System.Collections;
using Gameplay.Core;
using Gameplay.Interfaces;
using TMPro;
using UnityEngine;

namespace Gameplay.Interactable
{
    public class BloodVessel : MonoBehaviour, IPawn
    {
        [SerializeField] private ParticleSystem bloodEssence;
        [SerializeField] private Transform pointsTextContainer;
        [SerializeField] private TextMeshPro pointsText;

        private int _bloodPoints;
        private Transform _cameraTransform;
        
        private void Start()
        {
            StartCoroutine(StartParticleCor());
            _cameraTransform = Camera.main.transform;
            pointsText.enabled = false;
        }

        private void Update()
        {
            pointsTextContainer.LookAt(_cameraTransform.position);
        }

        private IEnumerator StartParticleCor()
        {
            yield return new WaitForSeconds(2);
            bloodEssence.Play();
            pointsText.enabled = true;
        }

        public void SetBloodPoints(int bloodPoints)
        {
            _bloodPoints = bloodPoints;
            pointsText.text = _bloodPoints.ToString();
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
