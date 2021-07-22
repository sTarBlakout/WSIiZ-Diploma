using System;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnAnimator : MonoBehaviour
    {
        private static readonly int RotateRight = Animator.StringToHash("RotateRight");
        private static readonly int RotateLeft = Animator.StringToHash("RotateLeft");
        private static readonly int Walk = Animator.StringToHash("Walk");

        [SerializeField] private Animator animator;

        public void AnimateRotationRight(bool animate)
        {
            animator.SetBool(RotateRight, animate);
        }
        
        public void AnimateRotationLeft(bool animate)
        {
            animator.SetBool(RotateLeft, animate);
        }

        public void AnimateWalk(bool animate)
        {
            animator.SetBool(Walk, animate);
        }

        public void FootL() { }
        
        public void FootR() { }
    }
}
