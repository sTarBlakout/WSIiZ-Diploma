using Gameplay.Environment;
using UnityEngine;
using Lean.Touch;

namespace Gameplay.Player
{
    public class InputManager : MonoBehaviour
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
            
            var cell = hitInfo.collider.GetComponent<Cell>();
            if (cell != null)
            {
                Debug.Log("Clicked on cell");
            }
        }
    }
}
