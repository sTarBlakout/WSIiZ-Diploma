using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Environment;
using Gameplay.Interactable;
using Gameplay.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Pawns
{
    public class PawnDamageable : MonoBehaviour, IDamageable
    {
        [Header("Particles")] 
        [SerializeField] private ParticleSystem onHitParticle;

        public Action OnDeath;
        
        private int _dmgRec;
        private List<BloodVessel> _vessels = new List<BloodVessel>();

        private PawnMover _mover;
        private PawnAnimator _animator;
        private PawnNormalData _data;
        private GameArea _gameArea;

        public void Init(PawnNormalData data, PawnAnimator animator, PawnMover mover, GameArea gameArea, Action deathCallback)
        {
            _data = data;
            _animator = animator;
            _mover = mover;
            _gameArea = gameArea;
            OnDeath = deathCallback;
        }

        public void PreDamage(IPawn attacker, Action onPrepared)
        {
            onPrepared += TryBlock;
            _mover.RotateTo(attacker.WorldPosition, onPrepared);
        }
        
        private void TryBlock()
        {
            _animator.AnimateBlock(true);
        }

        public void Damage(int value, Action<int> onDamageDealt)
        {
            var health = _data.BloodLevel;
            _dmgRec = value;
            _data.ModifyBloodLevelBy(-value);
            if (_data.BloodLevel == 0) _dmgRec = value + (health - value);
            onDamageDealt?.Invoke(_dmgRec);
        }
        
        public void PostDamage(Action onPostDamage)
        {
            StartCoroutine(PostDamageCoroutine(onPostDamage));
        }
        
        private IEnumerator PostDamageCoroutine(Action onPostDamage)
        {
            _animator.AnimateGetHit();
            onHitParticle.Play();
            SpawnBloodVessels();
            if (_data.BloodLevel == 0) OnDeath?.Invoke();
            yield return new WaitForSeconds(_data.AfterDamageDelay);
            _animator.AnimateBlock(false);
            onPostDamage?.Invoke();
        }

        private void SpawnBloodVessels()
        {
            var maxVesselCount = Mathf.Min(_dmgRec, 3);
            var vesselCount = Random.Range(1, maxVesselCount + 1);
            var tiles = default(List<GameAreaTile>);
            var vessels = new List<BloodVessel>();
            var remainingDmg = _dmgRec;
            
            var dist = 1;
            var filter = new GameArea.TilesFilter
            {
                excludeBlockedTiles = true,
                excludeTileWithSamePos = true
            };
            while (tiles == null || tiles.Count == 0)
            {
                tiles = _gameArea.GetTilesByDistance(transform.position, dist, filter);
                dist++;
            }
            
            vesselCount = Mathf.Min(vesselCount, tiles.Count + _vessels.Count);

            if (_vessels.Count != 0)
            {
                if (dist > 2)
                {
                    vesselCount = Mathf.Min(vesselCount, _vessels.Count);
                    vessels = new List<BloodVessel>(_vessels);
                    vessels = vessels.Take(vesselCount).ToList();
                    tiles.Clear();
                }
                else
                {
                    var v = 0;
                    var t = 0;
                    for (var i = 0; i < vesselCount; i++)
                    {
                        if (Random.Range(1, vesselCount + 1) > tiles.Count)
                        {
                            if (v < _vessels.Count) v++;
                            else t++;
                        }
                        else
                        {
                            if (t < tiles.Count) t++;
                            else v++;
                        }
                    }
                    tiles = tiles.Take(t).ToList();
                    vessels = _vessels.Take(v).ToList();
                }
            }
            else
            {
                vesselCount = Mathf.Min(vesselCount, tiles.Count);
                tiles = tiles.Take(vesselCount).ToList();
            }
            
            foreach (var vessel in vessels)
            {
                if (remainingDmg == 0) break;
                var bloodPoints = vessels.Last() == vessel ? remainingDmg : Random.Range(1, remainingDmg + 1);
                remainingDmg -= bloodPoints;
                vessel.AddBloodPoints(bloodPoints);
            }

            foreach (var tile in tiles)
            {
                if (remainingDmg == 0) break;
                var vessel = Instantiate(_data.BloodVesselPrefab).GetComponent<BloodVessel>();
                vessel.transform.position = tile.transform.position;
                var bloodPoints = tiles.Last() == tile ? remainingDmg : Random.Range(1, remainingDmg + 1);
                remainingDmg -= bloodPoints;
                vessel.AddBloodPoints(bloodPoints);
                tile.Enter(vessel);
                _vessels.Add(vessel);
                _gameArea.AddPawn(vessel.gameObject);
            }
        }
    }
}
