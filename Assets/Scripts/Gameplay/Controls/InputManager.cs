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
        private GameObject _player;
        private PawnMover _playerPawnMover;
        private Path _path;
        private Coroutine _waitPathCor;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _playerPawnMover = _player.GetComponent<PawnMover>();
            
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
            
            if (hitInfo.collider.GetComponent<GameArea>() != null)
            {
                _path.CreatePath(_playerPawnMover.transform.position, hitInfo.point);
                if (_waitPathCor != null) StopCoroutine(_waitPathCor);
                _waitPathCor = StartCoroutine(WaitGeneratedPath());
            }
        }

        private IEnumerator WaitGeneratedPath()
        {
            yield return new WaitUntil(() => _path.IsGenerated());

            var vectorPath = new List<Vector3>();
            for (int i = 0; i < _path.GetPathPointList().Count; i++) vectorPath.Add(_path.GetPathPointWorld(i));
            _playerPawnMover.MoveByPath(vectorPath);
        }
    }
}
