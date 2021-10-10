using Gameplay.Interfaces;
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
        
        [Header("Movement")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float waitAfterRotate;
        [SerializeField] private float waitAfterMove;

        [Header("Attacking")] 
        [SerializeField] private float afterDamageDelay;
        [SerializeField] private int attackDistance;

        [Header("Prefabs")] 
        [SerializeField] private GameObject bloodVesselPrefab;
        [SerializeField] private GameObject wayMoveLinePrefab;
        [SerializeField] private GameObject wayAttackLinePrefab;
        [SerializeField] private GameObject wayInteractLinePrefab;

        #region General

        public int TeamId => teamId;
        public int BloodLevel => bloodLevel;
        public int ActionsPerTurn => actionsPerTurn;

        public void ModifyBloodLevelBy(int value)
        {
            bloodLevel =  Mathf.Max(0, bloodLevel + value);
        }

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
