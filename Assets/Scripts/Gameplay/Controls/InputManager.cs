using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Interfaces;
using Gameplay.Сharacters;
using Lean.Touch;
using SimplePF2D;
using UnityEngine;

namespace Gameplay.Controls
{
    public class InputManager : MonoBehaviour
    {
        private PawnController _playerController;
        private Coroutine _waitPathCor;
        private Path _path;

        private OrderType _orderType;
        private IDamageable _damageable;

        private void Awake()
        {
            _playerController = GameObject.FindWithTag("Player").GetComponent<PawnController>();
            _path = new Path(FindObjectOfType<SimplePathFinding2D>());
        }

        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }
        
        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            if (!Physics.Raycast(finger.GetRay(), out var hitInfo, Mathf.Infinity) || finger.IsOverGui) return;
            
            // Clicked on map, process simple movement
            if (hitInfo.collider.GetComponent<GameArea>() != null)
            {
                _orderType = OrderType.Move;
                GeneratePathToPosition(hitInfo.point, OnPathGenerated);
                return;
            }

            // Clicked on enemy, process attack behavior
            _damageable = hitInfo.collider.transform.parent.GetComponent<IDamageable>();
            if (_damageable != null && _damageable.IsEnemyFor(_playerController))
            {
                _orderType = OrderType.Attack;
                GeneratePathToPosition(_damageable.Position, OnPathGenerated);
                return;
            }
        }

        private void OnReachedDestination()
        {
            switch (_orderType)
            {
                case OrderType.Attack: OrderRotate(_damageable.Position); break;
                default: _orderType = OrderType.None; break;
            }
        }

        private void OnRotated()
        {
            switch (_orderType)
            {
                case OrderType.Attack: OrderAttack(_damageable); break;
                default: _orderType = OrderType.None; break;
            }
        }
        
        private void OnPathGenerated(List<Vector3> path)
        {
            switch (_orderType)
            {
                case OrderType.Move: OrderSimpleMove(path); break;
                case OrderType.Attack: OrderMoveForAttacking(path); break;
                default: _orderType = OrderType.None; break;
            }
        }

        private void OrderRotate(Vector3 position)
        {
            _playerController.RotateTo(position, OnRotated);
        }

        private void OrderSimpleMove(List<Vector3> path)
        {
            _playerController.MovePath(path, OnReachedDestination);
        }
        
        private void OrderMoveForAttacking(List<Vector3> path)
        {
            path.RemoveAt(path.Count - 1);
            _playerController.MovePath(path, OnReachedDestination);
        }
        
        private void OrderAttack(IDamageable _damageable)
        {
            _playerController.AttackTarget(_damageable);
        }

        private void GeneratePathToPosition(Vector3 position, Action<List<Vector3>> onGeneratedPath)
        {
            _path.CreatePath(_playerController.transform.position, position);
            if (_waitPathCor != null) StopCoroutine(_waitPathCor);
            _waitPathCor = StartCoroutine(WaitGeneratedPath(onGeneratedPath));
        }

        private IEnumerator WaitGeneratedPath(Action<List<Vector3>> onGeneratedPath)
        {
            yield return new WaitUntil(() => _path.IsGenerated());
            var vectorPath = new List<Vector3>();
            for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
            onGeneratedPath(vectorPath);
        }
    }
}
