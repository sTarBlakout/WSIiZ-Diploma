using UnityEngine;

namespace Gameplay.Сharacters
{
    [CreateAssetMenu(fileName = "PawnData", menuName = "Data/Pawn Data")]
    public class PawnData : ScriptableObject
    {
        [SerializeField] private int teamId;
        
        public int TeamId => teamId;
    }
}
