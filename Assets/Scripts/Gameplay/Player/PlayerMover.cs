using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float acceleration;
        
        private List<Vector3> _pathToCell;
        private float _currSpeed;

        private void Update()
        {
            ProcessMovement();
        }
        
        public void MoveToCell(List<Vector3> path)
        {
            _pathToCell = new List<Vector3>(path);
        }

        private void ProcessMovement()
        {
            if (_pathToCell == null) return;

            if (_pathToCell.Count != 0)
            {
                var currTarget = _pathToCell[0];
                if (currTarget != transform.position)
                {
                    var direction = currTarget - transform.position;
                    if (Vector3.Angle(transform.forward, direction) < 0.1f)
                    {
                        Move(currTarget);
                    }
                    else
                    {
                        Rotate(direction);
                    }
                }
                else
                {
                    _pathToCell.RemoveAt(0);
                    _currSpeed = 0f;
                }
            }
            else
            {
                _pathToCell = null;
            }
        }

        private void Rotate(Vector3 direction)
        {
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        private void Move(Vector3 position)
        {
            if (Vector3.Distance(transform.position, position) < 1f)
                _currSpeed -= Mathf.Min(acceleration * Time.deltaTime, 1);
            else
                _currSpeed += Mathf.Min(acceleration * Time.deltaTime, 1);  

            transform.position = Vector3.MoveTowards(transform.position, position, maxMoveSpeed * _currSpeed * Time.deltaTime);
        }
    }
}
