using UnityEngine;

namespace Gameplay.Сharacters
{
    [CreateAssetMenu(fileName = "PawnData", menuName = "Data/Pawn Data")]
    public class PawnData : ScriptableObject
    {
        [SerializeField] private int teamId;
        [SerializeField] private int level;
        
        public int TeamId => teamId;
        public int Damage => level;
    }
}
