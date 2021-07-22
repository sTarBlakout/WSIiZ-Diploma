using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnAttacker : MonoBehaviour
    {
        private PawnAnimator _pawnAnimator;
        
        public void Init(PawnAnimator pawnAnimator)
        {
            _pawnAnimator = pawnAnimator;
        }
    }
}
