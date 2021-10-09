using System;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Environment
{
    public class GameAreaWay : MonoBehaviour
    {
        private LineRenderer _wayLine;
        private LineRenderer _attackLine;
        private LineRenderer _interactLine;
        private Transform _followPawn;
        private List<GameAreaTile> _path;
        private OrderType _orderType = OrderType.None;

        public List<GameAreaTile> Path => _path;

        private void Update()
        {
            if (_orderType == OrderType.None) return;
            
            if (_followPawn != null && _wayLine != null)
            {
                _wayLine.SetPosition(0, _followPawn.position);
                if (_wayLine.positionCount > 1 && _wayLine.GetPosition(0) == _wayLine.GetPosition(1))
                {
                    for (int i = 0; i < _wayLine.positionCount - 1; i++)
                        _wayLine.SetPosition(i, _wayLine.GetPosition(i + 1));
                    _wayLine.positionCount--;
                    if (_wayLine.positionCount == 1 && _orderType == OrderType.Move) DestroyWay();
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

        public GameAreaWay SetAttackLine(GameObject atkLinePrefab)
        {
            _attackLine = Instantiate(atkLinePrefab).GetComponent<LineRenderer>();
            _attackLine.transform.parent = transform;
            _attackLine.enabled = false;
            
            return this;
        }
        
        public GameAreaWay SetInteractableLine(GameObject intLinePrefab)
        {
            _interactLine = Instantiate(intLinePrefab).GetComponent<LineRenderer>();
            _interactLine.transform.parent = transform;
            _interactLine.enabled = false;
            
            return this;
        }

        public GameAreaWay BuildWayToPawn(IPawn target)
        {
            var line = target is IPawnNormal ? _attackLine : _interactLine;
            
            line.positionCount = 2;
            if (_wayLine != null && _wayLine.positionCount > 1)
            {
                line.SetPosition(0, _wayLine.GetPosition(_wayLine.positionCount - 1));
            }
            else
            {
                line.SetPosition(0, _followPawn.position);
            }
            
            line.SetPosition(1, target.WorldPosition);
            line.enabled = true;
            
            return this;
        }

        public GameAreaWay SetFollowPawn(Transform followPawn)
        {
            _followPawn = followPawn;
            return this;
        }
        
        public GameAreaWay SetOrder(OrderType orderType)
        {
            _orderType = orderType;
            return this;
        }

        public void DestroyWay()
        {
            Destroy(gameObject);
        }
    }
}
