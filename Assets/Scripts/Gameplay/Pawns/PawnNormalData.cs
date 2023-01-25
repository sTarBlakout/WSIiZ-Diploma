using System.Collections.Generic;
using Gameplay.Interfaces;
using Gameplay.Items;
using Global;
using UnityEngine;

namespace Gameplay.Pawns
{
    [CreateAssetMenu(fileName = "NormalPawnData", menuName = "Data/Normal Pawn Data")]
    public class PawnNormalData : PawnData, IPawnNormalData
    {
        [Header("General")]
        [SerializeField] private int teamId;
        [SerializeField] private int bloodLevel;
        [SerializeField] private int actionsPerTurn;
        [SerializeField] private int distancePerTurn;
        [SerializeField] private List<GameObject> items;
        [SerializeField] private AnimatorOverrideController animatorOverrideController;
        
        [Header("Movement")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float waitAfterRotate;
        [SerializeField] private float waitAfterMove;

        [Header("Attacking")] 
        [SerializeField] private float afterDamageDelay;
        [SerializeField] private int attackDistance;

        [Header("Audio")] 
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip interactSound;
        [SerializeField] private AudioClip movingSound;

        [Header("Prefabs")] 
        [SerializeField] private GameObject bloodVesselPrefab;
        [SerializeField] private GameObject wayMoveLinePrefab;
        [SerializeField] private GameObject wayAttackLinePrefab;
        [SerializeField] private GameObject wayInteractLinePrefab;

        #region General

        public int TeamId => teamId;
        public int BloodLevel => bloodLevel;
        public int ActionsPerTurn => actionsPerTurn;
        public List<GameObject> Items => items;
        public AnimatorOverrideController AnimatorOverrideController => animatorOverrideController;

        public void Init()
        {
            if (teamId == 1)
            {
                var playerCharacterPrefs = GlobalManager.Instance.GlobalData.LoadPlayerCharacterPrefs();
                if (playerCharacterPrefs.BloodLevel > 0)
                {
                    bloodLevel = playerCharacterPrefs.BloodLevel;
                }
                
                items.AddRange(playerCharacterPrefs.Items);
            }
            else
            {
                bloodLevel = (int) (bloodLevel * GlobalManager.Instance.GlobalData.RepeatLevelEnemyBloodModifier);
            }
        }
        
        public void ModifyBloodLevelBy(int value)
        {
            bloodLevel =  Mathf.Max(0, bloodLevel + value);
            if (teamId == 1) GlobalManager.Instance.GlobalData.SavePlayerCharacterBloodLevel(bloodLevel);
        }

        #endregion

        #region Audio

        public AudioClip HitSound => hitSound;
        public AudioClip DeathSound => deathSound;
        public AudioClip InteractSound => interactSound;
        public AudioClip MovingSound => movingSound;

        #endregion
        
        #region Movement

        public float RotationSpeed => rotationSpeed;
        public float MovementSpeed => movementSpeed;
        public float WaitAfterRotate => waitAfterRotate;
        public float WaitAfterMove => waitAfterMove;
        public int DistancePerTurn => distancePerTurn;

        #endregion

        #region Attacking

        public float AfterDamageDelay => afterDamageDelay;
        public int AttackDistance => attackDistance;

        #endregion

        #region Prefabs

        public GameObject BloodVesselPrefab => bloodVesselPrefab;
        public GameObject WayMoveLinePrefab => wayMoveLinePrefab;
        public GameObject WayAttackLinePrefab => wayAttackLinePrefab;
        public GameObject WayInteractLinePrefab => wayInteractLinePrefab;

        #endregion
    }
}
