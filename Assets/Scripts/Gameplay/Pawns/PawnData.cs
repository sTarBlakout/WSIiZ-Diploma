using UnityEngine;

namespace Gameplay.Pawns
{
    [CreateAssetMenu(fileName = "PawnData", menuName = "Data/Pawn Data")]
    public class PawnData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private int teamId;
        [SerializeField] private int level;
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
        [SerializeField] private GameObject wayMoveLinePrefab;
        [SerializeField] private GameObject wayAttackLinePrefab;

        #region General

        public int TeamId => teamId;
        public int Level => level;
        public int ActionsPerTurn => actionsPerTurn;

        public void ModifyLevelBy(int value)
        {
            level =  Mathf.Max(0, level + value);
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
        public int Damage => level;

        #endregion

        #region Prefabs

        public GameObject WayMoveLinePrefab => wayMoveLinePrefab;
        public GameObject WayAttackeLinePrefab => wayAttackLinePrefab;

        #endregion
    }
}
