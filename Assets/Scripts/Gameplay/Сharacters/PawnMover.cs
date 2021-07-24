using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnMover : MonoBehaviour
    {
        private PawnData _pawnData;
        private GameObject _charGraphics;
        
        private Coroutine _waitCoroutine;
        private bool _waitedAfterRotate;
        private bool _waitedAfterMove;
        
        private List<(Vector3 pos, bool rot)> _vectorPath;
        private PawnAnimator _pawnAnimator;
        private Vector3 _rotateToPos;

        private Action _onReachedDestination;
        private Action _onRotated;

        private void Update()
        {
            if (IsWaiting()) return;
            
            ProcessTraversing();
            ProcessRotation();
        }

        public void Init(PawnData pawnData, PawnAnimator pawnAnimator, GameObject charGraphics)
        {
            _pawnData = pawnData;
            _pawnAnimator = pawnAnimator;
            _charGraphics = charGraphics;
        }
        
        public void RotateTo(Vector3 position, Action onRotated)
        {
            _rotateToPos = position;
            _onRotated = onRotated;
        }
        
        public void MovePath(List<Vector3> path, Action onReachedDestination)
        {
            _onReachedDestination = onReachedDestination;
            _vectorPath = TraversePath(path);
            TargetNextPoint();
        }

        private List<(Vector3 pos, bool rot)> TraversePath(List<Vector3> path)
        {
            var rez = new List<(Vector3 pos, bool rot)>();

            var traverser = new GameObject("Traverser");
            traverser.transform.position = transform.position;
            traverser.transform.rotation = transform.rotation;
            
            foreach (var point in path)
            {
                (Vector3 pos, bool rot) posRot = (point, false);
                if (Vector3.Angle(traverser.transform.forward, point - traverser.transform.position) > 5f)
                {
                    rez[rez.Count - 1] = (rez[rez.Count - 1].pos, true);
                    traverser.transform.LookAt(point);
                }
            
                traverser.transform.position = point;
                rez.Add(posRot);
            }
            
            rez[rez.Count - 1] = (rez[rez.Count - 1].pos, true);
            rez.RemoveAll(posRot => !posRot.rot);
            Destroy(traverser);
            
            return rez;
        }

        private void ProcessRotation()
        {
            if (_onRotated == null) return;
            if (Rotate(_rotateToPos)) return;
            
            if (!_waitedAfterRotate)
            {
                InitWaiting(_pawnData.WaitAfterRotate, () => _waitedAfterRotate = true);
                return;
            }
            
            _onRotated?.Invoke();
            _onRotated = null;
        }

        private void ProcessTraversing()
        {
            if (_vectorPath == null) return;

            if (_vectorPath.Count != 0)
            {
                if (Rotate(_vectorPath[0].pos)) return;
                if (!_waitedAfterRotate)
                {
                    InitWaiting(_pawnData.WaitAfterRotate, () => _waitedAfterRotate = true);
                    return;
                }
                
                if (Move(_vectorPath[0].pos)) return;
                if (!_waitedAfterMove)
                {
                    InitWaiting(_pawnData.WaitAfterMove,  () => _waitedAfterMove = true);
                    return;
                }
                
                ResetAllWaitings();
                TargetNextPoint();
            }
            else
            {
                _vectorPath = null;
            }
        }

        private bool IsWaiting()
        {
            return _waitCoroutine != null;
        }

        private void InitWaiting(float waitTime, Action onWaitEnd)
        {
            if (_waitCoroutine != null) StopCoroutine(_waitCoroutine);
            _waitCoroutine = StartCoroutine(WaitForSeconds(waitTime, onWaitEnd));
        }

        private void ResetAllWaitings()
        {
            _waitedAfterMove = false;
            _waitedAfterRotate = false;
            _waitCoroutine = null;
        }

        private IEnumerator WaitForSeconds(float waitTime, Action onWaitEnd)
        {
            yield return new WaitForSeconds(waitTime);
            _waitCoroutine = null;
            onWaitEnd?.Invoke();
        }

        private void TargetNextPoint()
        {
            _vectorPath.RemoveAt(0);
            if (_vectorPath.Count == 0) _onReachedDestination?.Invoke();
        }

        private bool Rotate(Vector3 position)
        {
            var direction = position - transform.position;
            var angle = Vector3.Angle(_charGraphics.transform.forward, direction);

            if (angle > 1f) 
            {
                if (Vector3.Dot(direction, _charGraphics.transform.right) > 0)
                    _pawnAnimator.AnimateMovement(AnimMovement.RotateRight, true);
                else
                    _pawnAnimator.AnimateMovement(AnimMovement.RotateLeft, true);

                var lookRotation = Quaternion.LookRotation(direction);
                _charGraphics.transform.rotation = Quaternion.RotateTowards(
                    _charGraphics.transform.rotation, lookRotation, _pawnData.RotationSpeed * Time.deltaTime);
                return true;
            }
            
            _pawnAnimator.AnimateMovement(AnimMovement.RotateLeft, false);
            _pawnAnimator.AnimateMovement(AnimMovement.RotateRight, false);
            
            _charGraphics.transform.LookAt(position);
            return false;
        }

        private bool Move(Vector3 postion)
        {
            transform.position = Vector3.MoveTowards(transform.position, postion, _pawnData.MovementSpeed * Time.deltaTime);
            _pawnAnimator.AnimateMovement(AnimMovement.Walk, postion != transform.position);
            return postion != transform.position;
        }
        
        #region Animation Events
        
        public void FootL() { }
        
        public void FootR() { }
        
        #endregion
    }
}
