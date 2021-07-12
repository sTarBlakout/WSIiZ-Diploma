using Gameplay.Environment;
using Gameplay.Сharacters;
using Lean.Touch;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManager : MonoBehaviour
    {
        private GameObject _player;
        private PawnMover _playerPawnMover;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _playerPawnMover = _player.GetComponent<PawnMover>();
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

            var cell = hitInfo.collider.transform.parent.GetComponent<Cell>();
            if (cell != null) _playerPawnMover.MoveToCell(cell.GetPathToCell());
        }
    }
}
