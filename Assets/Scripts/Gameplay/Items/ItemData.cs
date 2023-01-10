using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject, IItemData
    {
        [SerializeField] private int itemId;
        [SerializeField] private string itemName;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private ItemType itemType;
        [SerializeField] private AnimatorOverrideController overrideController;

        public int ItemId => itemId;
        public string ItemName => itemName;
        public Sprite ItemIcon => itemIcon;
        public ItemType ItemType => itemType;
        public AnimatorOverrideController OverrideController => overrideController;
    }
}
