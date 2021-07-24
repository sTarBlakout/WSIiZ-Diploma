using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnAnimator : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");
        private static readonly int Block = Animator.StringToHash("Block");

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
            animator.SetTrigger(GetHit);
        }

        public void AnimateBlock(bool animate)
        {
            animator.SetBool(Block, animate);
        }
    }
}
