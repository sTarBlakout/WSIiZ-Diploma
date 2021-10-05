using System;
using System.Collections;
using Gameplay.Core;
using Gameplay.Interfaces;
using TMPro;
using UnityEngine;

namespace Gameplay.Interactable
{
    public class BloodVessel : MonoBehaviour, IPawnInteractable, IPawnData
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

        public void AddBloodPoints(int bloodPoints)
        {
            _bloodPoints += bloodPoints;
            pointsText.text = _bloodPoints.ToString();
        }

        #region IPawn Implementation
        
        public PawnRelation RelationTo(IPawn pawn)
        {
            return PawnRelation.Interactable;
        }
        
        public bool IsBlockingTile => true;
        public Vector3 WorldPosition => transform.position;
        public IPawnData PawnData => this;

        #endregion
        
        #region IPawn Data
        
        public Vector3 Position => transform.position;
        public int TeamId { get; }
        public int DamageValue { get; }
        public void ModifyLevelBy(int value) { }

        #endregion

        #region IPawnInteractable Implementation

        private IPawn _interactor;
        
        public void PreInteract(IPawn interactor, Action onPreInteract)
        {
            _interactor = interactor;
            onPreInteract?.Invoke();
        }

        public void Interact(Action onInteract)
        {
            _interactor.PawnData.ModifyLevelBy(_bloodPoints);
            onInteract?.Invoke();
        }

        public void PostInteract(Action onPostInteract)
        {
            onPostInteract?.Invoke();  
        }

        #endregion
    }
}
