using System;
using Gameplay.Environment;
using Gameplay.Player;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManager : MonoBehaviour
    {
        private PlayerMover _playerMover;

        private void Awake()
        {
            _playerMover = FindObjectOfType<PlayerMover>();
        }

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
            
            var cell = hitInfo.collider.GetComponent<Cell>();
            if (cell != null) _playerMover.MoveToCell(cell.GetPathToCell());
        }
    }
}
