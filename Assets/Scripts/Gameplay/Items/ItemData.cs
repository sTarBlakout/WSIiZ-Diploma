using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject, IItemData
    {
        [SerializeField] private ItemType itemType;
        
        public ItemType ItemType => itemType;
    }
}
