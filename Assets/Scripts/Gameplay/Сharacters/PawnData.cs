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

        [Header("Attacking")] 
        [SerializeField] private float afterDamageDelay;

        #region General

        public int TeamId => teamId;
        public int Damage => level;
        public int Health => level;
        
        #endregion

        #region Movement

        public float RotationSpeed => rotationSpeed;
        public float MovementSpeed => movementSpeed;
        public float WaitAfterRotate => waitAfterRotate;
        public float WaitAfterMove => waitAfterMove;

        #endregion

        #region Attacking

        public float AfterDamageDelay => afterDamageDelay;

        #endregion
    }
}
