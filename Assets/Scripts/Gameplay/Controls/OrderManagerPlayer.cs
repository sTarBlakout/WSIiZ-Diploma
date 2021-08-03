using Gameplay.Core;
using Gameplay.Interfaces;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class OrderManagerPlayer : OrderManagerBase
    {
        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }
        
        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;
            
            // Clicked on map, process simple movement
            if (hitInfo.collider.GetComponent<GameArea>() != null)
            {
                _order = Order.Move;
                _gameArea.GeneratePathToPosition(_pawnController.transform.position, hitInfo.point, OnPathGenerated);
                return;
            }
            
            // Checking if pawn
            _interactable = hitInfo.collider.transform.parent.GetComponent<IInteractable>();
            if (_interactable != null)
            {
                if (_interactable is IDamageable damageable) _damageable = damageable;

                // Clicked on enemy, try attack
                if (_damageable != null && _damageable.IsInteractable() && _damageable.IsEnemyFor(_pawnController))
                {
                    _order = Order.Attack;
                    _gameArea.GeneratePathToPosition(_pawnController.transform.position, _damageable.Position, OnPathGenerated);
                    return;
                }
            }
        }
    }
}
