using UnityEngine;

namespace Gameplay.Сharacters
{
    [CreateAssetMenu(fileName = "PawnData", menuName = "Data/Pawn Data")]
    public class PawnData : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private int teamId;
        [SerializeField] private int level;
        
        [Header("Movement")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float waitAfterRotate;
        [SerializeField] private float waitAfterMove;
        [SerializeField] private int distancePerTurn;

        [Header("Attacking")] 
        [SerializeField] private float afterDamageDelay;
        [SerializeField] private int actionsPerTurn;

        #region General

        public int TeamId => teamId;
        public int Damage => level;
        public int Level => level;

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
        public int ActionsPerTurn => actionsPerTurn;

        #endregion
    }
}
