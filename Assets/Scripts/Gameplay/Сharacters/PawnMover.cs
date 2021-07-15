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
        
        private List<Vector3> _vectorPath;
        private float _currSpeed;
        private float _totalDistToNextPoint;

        private void Update()
        {
            ProcessMovement();
        }
        
        public void MoveByPath(List<Vector3> path)
        {
            _vectorPath = new List<Vector3>(path);
            TargetNextPoint(true);
        }

        private void ProcessMovement()
        {
            if (_vectorPath == null) return;

            if (_vectorPath.Count != 0)
            {
                if (Rotate(_vectorPath[0])) return;
                if (Move(_vectorPath[0])) return;
                TargetNextPoint();
            }
            else
            {
                _vectorPath = null;
            }
        }

        private void TargetNextPoint(bool first = false)
        {
            if (!first) _vectorPath.RemoveAt(0);
            if (_vectorPath.Count == 0) return;
            _totalDistToNextPoint = Vector3.Distance(transform.position, _vectorPath[0]);
            _currSpeed = 0f;
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

        private bool Move(Vector3 position)
        {
            if (Vector3.Distance(transform.position, position) < _totalDistToNextPoint / 2)
                _currSpeed -= Mathf.Min(acceleration * Time.deltaTime, 1);
            else
                _currSpeed += Mathf.Min(acceleration * Time.deltaTime, 1);

            _currSpeed = Mathf.Clamp(_currSpeed, 0f, 1f);
            transform.position = Vector3.MoveTowards(transform.position, position, maxMoveSpeed * _currSpeed * Time.deltaTime);

            if (Vector3.SqrMagnitude(position - transform.position) < 0.001f) transform.position = position;
            return position != transform.position;
        }
    }
}
