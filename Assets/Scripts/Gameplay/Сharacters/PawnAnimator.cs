using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnAnimator : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");
        private static readonly int GetHitBlocked = Animator.StringToHash("GetHitBlocked");
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int Die = Animator.StringToHash("Die");

        [SerializeField] private Animator animator;

        public void AnimateMovement(AnimMovement movement, bool animate)
        {
            animator.SetBool(movement.ToString(), animate);
        }
        
        public void AnimateAttack()
        {
            animator.SetTrigger(Attack);
        }
        
        public void AnimateGetHit()
        {
            animator.SetTrigger(animator.GetBool(Block) ? GetHitBlocked : GetHit);
        }

        public void AnimateBlock(bool animate)
        {
            animator.SetBool(Block, animate);
        }

        public void AnimateDie()
        {
            animator.SetTrigger(Die);
        }
    }
}
