using System;
using System.Collections;
using System.Collections.Generic;
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
                GeneratePathToPosition(hitInfo.point, OrderMovePath);
                return;
            }

            // Clicked on enemy, process attack behavior
            var otherPawn = hitInfo.collider.transform.parent.GetComponent<PawnController>();
            if (otherPawn != null && otherPawn.IsEnemyFor(_playerController))
            {
                GeneratePathToPosition(hitInfo.point, OrderMovePath);
            }
        }

        private void OrderMovePath(List<Vector3> path)
        {
            _playerController.MovePath(path, () => {});
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
