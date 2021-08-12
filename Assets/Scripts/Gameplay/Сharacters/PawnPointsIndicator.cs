using TMPro;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnPointsIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshPro movePointsText;
        [SerializeField] private TextMeshPro actionPointsText;
        
        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
            Show(false);
        }

        private void Update()
        {
            transform.LookAt(_cameraTransform.position);
        }

        public PawnPointsIndicator Show(bool show)
        {
            movePointsText.enabled = show;
            actionPointsText.enabled = show;
            return this;
        }
        
        public PawnPointsIndicator SetMovePoints(int value)
        {
            movePointsText.text = value.ToString();
            return this;
        }
        
        public PawnPointsIndicator SetActionPoints(int value)
        {
            actionPointsText.text = value.ToString();
            return this;
        }
    }
}
