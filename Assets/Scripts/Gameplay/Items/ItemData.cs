using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject, IItemData
    {
        [SerializeField] private string itemName;
        [SerializeField] private ItemType itemType;

        public string ItemName => itemName;
        public ItemType ItemType => itemType;
    }
}
