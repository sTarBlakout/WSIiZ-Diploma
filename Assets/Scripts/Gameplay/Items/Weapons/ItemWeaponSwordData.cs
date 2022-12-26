using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items.Weapons
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Weapon Data")]
    public class ItemWeaponSwordData : ItemData, IItemWeaponData
    {
        [SerializeField] private int damageModifier;
        [SerializeField] private int rangeModifier;

        public int DamageModifier => damageModifier;
        public int RangeModifier => rangeModifier;
    }
}
