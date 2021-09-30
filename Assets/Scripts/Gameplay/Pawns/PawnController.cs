using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Environment;
using Gameplay.Interactable;
using Gameplay.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Pawns
{
    public class PawnController : MonoBehaviour, IPawn, IPawnData, IDamageable
    {
        [Header("Logic Components")]
        [SerializeField] private PawnData pawnData;
        [SerializeField] private PawnAnimator pawnAnimator;
        [SerializeField] private PawnAttacker pawnAttacker;
        [SerializeField] private PawnMover pawnMover;
        
        [Header("Indicators")]
        [SerializeField] private PawnHealthIndicator pawnHealthIndicator;
        [SerializeField] private PawnPointsIndicator pawnPointsIndicator;
        
        [Header("Other Components")]
        [SerializeField] private GameObject pawnGraphics;
        [SerializeField] private Collider pawnCollider;

        [Header("Particles")] 
        [SerializeField] private ParticleSystem onHitParticle;

        private PawnData _currPawnData;
        private GameArea _gameArea;
        
        public PawnData Data => _currPawnData;
        public PawnPointsIndicator PointsIndicator => pawnPointsIndicator;
        
        public Action onDeath;

        private void Awake()
        {
            pawnCollider.enabled = false;
        }

        public void Init()
        {
            pawnCollider.enabled = true;
            _currPawnData = Instantiate(pawnData);
            _gameArea = FindObjectOfType<GameArea>();

            pawnMover.Init(_currPawnData, pawnAnimator, pawnGraphics);
            pawnAttacker.Init(this, pawnAnimator);
            pawnHealthIndicator.Init(_currPawnData);
        }

        public void MovePath(List<Vector3> path, Action onReachedDestination)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else
                pawnMover.MovePath(path, onReachedDestination);
        }
        
        public void RotateTo(Vector3 position, Action onRotated)
        {
            if (pawnMover == null)
                Debug.LogError($"{gameObject.name} does not have any mover component!");
            else
                pawnMover.RotateTo(position, onRotated);
        }

        public void AttackTarget(IPawn target, Action onAttacked)
        {
            if (pawnAttacker == null)
                Debug.LogError($"{gameObject.name} does not have any attacker component!");
            else
                pawnAttacker.AttackTarget(target, onAttacked);
        }

        private IEnumerator DeathCoroutine()
        {
            pawnAnimator.AnimateDie();
            pawnCollider.enabled = false;
            pawnMover.enabled = false;
            yield return new WaitForSeconds(1f);
            pawnHealthIndicator.Show(false);
            onDeath?.Invoke();
        }

        #region IPawn Implementation

        public bool IsBlockingTile => IsAlive;
        public bool IsAlive => _currPawnData.Level != 0;
        public IPawnData PawnData => this;
        public IDamageable Damageable => this;

        public PawnRelation RelationTo(IPawn pawn)
        {
            return pawn.PawnData.TeamId == _currPawnData.TeamId ? PawnRelation.Friend : PawnRelation.Enemy;
        }
        

        #endregion
        
        #region IPawnData Implementation

        public Vector3 Position => transform.position;
        public int TeamId => _currPawnData.TeamId;
        public int DamageValue => _currPawnData.Damage;
        public void ModifyLevelBy(int value) { _currPawnData.ModifyLevelBy(value); }

        #endregion
        
        #region IDamageable Implementation

        private int _dmgRec;
        private List<BloodVessel> _vessels = new List<BloodVessel>();

        public void PreDamage(IPawn attacker, Action onPrepared)
        {
            onPrepared += TryBlock;
            RotateTo(attacker.PawnData.Position, onPrepared);
        }
        
        private void TryBlock()
        {
            pawnAnimator.AnimateBlock(true);
        }

        public void Damage(int value, Action<int> onDamageDealt)
        {
            var health = _currPawnData.Level;
            _dmgRec = value;
            _currPawnData.ModifyLevelBy(-value);
            if (_currPawnData.Level == 0) _dmgRec = value + (health - value);
            onDamageDealt?.Invoke(_dmgRec);
        }
        
        public void PostDamage(Action onPostDamage)
        {
            StartCoroutine(PostDamageCoroutine(onPostDamage));
        }
        
        private IEnumerator PostDamageCoroutine(Action onPostDamage)
        {
            pawnAnimator.AnimateGetHit();
            onHitParticle.Play();
            SpawnBloodVessels();
            if (_currPawnData.Level == 0) StartCoroutine(DeathCoroutine());
            yield return new WaitForSeconds(pawnData.AfterDamageDelay);
            pawnAnimator.AnimateBlock(false);
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
                var vessel = Instantiate(Data.BloodVesselPrefab).GetComponent<BloodVessel>();
                vessel.transform.position = tile.transform.position;
                var bloodPoints = tiles.Last() == tile ? remainingDmg : Random.Range(1, remainingDmg + 1);
                remainingDmg -= bloodPoints;
                vessel.AddBloodPoints(bloodPoints);
                tile.Enter(vessel);
                _vessels.Add(vessel);
            }
        }

        #endregion
    }
}
