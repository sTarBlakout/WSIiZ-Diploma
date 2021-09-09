using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaWay : MonoBehaviour
    {
        private LineRenderer _wayLine;
        private Transform _followPawn;
        private List<GameAreaTile> _path;

        public List<GameAreaTile> Path => _path;

        private void Update()
        {
            if (_followPawn != null)
            {
                _wayLine.SetPosition(0, _followPawn.position);
                if (_wayLine.positionCount > 1 && _wayLine.GetPosition(0) == _wayLine.GetPosition(1))
                {
                    for (int i = 0; i < _wayLine.positionCount - 1; i++)
                        _wayLine.SetPosition(i, _wayLine.GetPosition(i + 1));
                    _wayLine.positionCount--;
                    if (_wayLine.positionCount == 1) DestroyWay();
                }
            }
        }

        public GameAreaWay SetWayLine(GameObject wayLinePrefab)
        {
            _wayLine = Instantiate(wayLinePrefab).GetComponent<LineRenderer>();
            _wayLine.transform.parent = transform;
            _wayLine.enabled = false;
            return this;
        }
        
        public GameAreaWay BuildWay(List<GameAreaTile> path)
        {
            _path = path;
            var idx = 0;
            _wayLine.positionCount = path.Count;
            foreach (var tile in path)
            {
                _wayLine.SetPosition(idx, tile.WorldPos);
                idx++;
            }

            _wayLine.enabled = true;
            return this;
        }

        public GameAreaWay SetFollowPawn(Transform followPawn)
        {
            _followPawn = followPawn;
            return this;
        }

        public void DestroyWay()
        {
            Destroy(gameObject);
        }
    }
}
