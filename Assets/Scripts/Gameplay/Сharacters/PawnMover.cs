using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Сharacters
{
    public class PawnMover : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float acceleration;

        private List<(Vector3 pos, bool rot)> _vectorPath;
        private Vector3 _prevPos;
        private float _currSpeed;
        private float _totalDistToNextPoint;
        private float _distReachedFullSpeed;

        private void Update()
        {
            ProcessMovement();
        }
        
        public void MoveByPath(List<Vector3> path)
        {
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

        private void ProcessMovement()
        {
            if (_vectorPath == null) return;

            if (_vectorPath.Count != 0)
            {
                if (Rotate(_vectorPath[0].pos)) return;
                if (Move(_vectorPath[0].pos)) return;
                TargetNextPoint();
            }
            else
            {
                _vectorPath = null;
            }
        }

        private void TargetNextPoint()
        {
            _prevPos = _vectorPath[0].pos;
            _vectorPath.RemoveAt(0);
            if (_vectorPath.Count == 0) return;
            _totalDistToNextPoint = Vector3.Distance(transform.position, _vectorPath[0].pos);
            _currSpeed = 0f;
            _distReachedFullSpeed = -1f;
        }

        private bool Rotate(Vector3 position)
        {
            var direction = position - transform.position;
            
            if (Vector3.Angle(transform.forward, direction) > 1f) 
            {
                var lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                return true;
            }
            
            transform.LookAt(position);
            return false;
        }

        private bool Move(Vector3 postion)
        {
            var remainDist = Vector3.Distance(transform.position, postion);

            if (_distReachedFullSpeed == -1f)
            {
                if (remainDist < _totalDistToNextPoint / 2)
                    _currSpeed -= Mathf.Min(acceleration * Time.deltaTime, 1);
                else
                    _currSpeed += Mathf.Min(acceleration * Time.deltaTime, 1);
            }
            else
            {
                if (_distReachedFullSpeed > remainDist)
                {
                    _currSpeed -= Mathf.Min(acceleration * Time.deltaTime, 1);
                }
            }
            
            _currSpeed = Mathf.Clamp(_currSpeed, 0.1f, 1f);
            transform.position = Vector3.MoveTowards(transform.position, postion, maxMoveSpeed * _currSpeed * Time.deltaTime);
            if (_currSpeed == 1f && _distReachedFullSpeed == -1) _distReachedFullSpeed = Vector3.Distance(transform.position, _prevPos);

            if (Vector3.SqrMagnitude(postion - transform.position) < 0.001f) transform.position = postion;
            return postion!= transform.position;
        }
    }
}
