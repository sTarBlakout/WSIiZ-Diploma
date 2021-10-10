using TMPro;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnHealthIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshPro levelText;
        
        private PawnNormalData _pawnData;
        private Transform _cameraTransform;

        public void Init(PawnNormalData pawnData)
        {
            _pawnData = pawnData;
            _cameraTransform = Camera.main.transform;
        }
        
        private void Update()
        {
            if (_pawnData == null) return;
            levelText.text = _pawnData.BloodLevel.ToString(); 
            transform.LookAt(_cameraTransform.position);
        }

        public void Show(bool show)
        {
            levelText.enabled = show;
        }
    }
}
