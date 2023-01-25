using System;
using Gameplay.Interfaces;
using Global;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnInteractor : MonoBehaviour
    {
        private IPawnNormal _pawn;
        
        private IPawnInteractable _target;
        private Action _onInteracted;

        public void Init(IPawnNormal pawn)
        {
            _pawn = pawn;
        }

        public void InteractWithTarget(IPawnInteractable target, Action onInteracted)
        {
            _target = target;
            _onInteracted = onInteracted;
            _target.PreInteract(_pawn, OnPreInteracted);
        }
        
        private void OnPreInteracted()
        {
            _target.Interact(OnInteracted);
        }
        
        private void OnInteracted()
        {
            _target.PostInteract(OnPostInteracted);
            AudioManager.Instance.PlaySound(_pawn.PawnData.InteractSound);
        }
        
        private void OnPostInteracted()
        {
            _onInteracted?.Invoke();
        }
    }
}
