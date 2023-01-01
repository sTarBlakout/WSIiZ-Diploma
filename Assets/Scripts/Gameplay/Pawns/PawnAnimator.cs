using System;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnAnimator : MonoBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int GetHit = Animator.StringToHash("GetHit");
        private static readonly int GetHitBlocked = Animator.StringToHash("GetHitBlocked");
        private static readonly int Block = Animator.StringToHash("Block");
        private static readonly int Die = Animator.StringToHash("Die");

        [SerializeField] private Animator animator;

        private AnimatorOverrideController _defaultAnimatorController;

        private void Awake()
        {
            _defaultAnimatorController = animator.runtimeAnimatorController as AnimatorOverrideController;
        }

        public void OverrideController(AnimatorOverrideController overrideController)
        {
            animator.runtimeAnimatorController = overrideController == null ? _defaultAnimatorController : overrideController;
        }
        
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
