using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items.Weapons
{
    public class ItemWeaponSword : MonoBehaviour, IItemWeapon
    {
        [SerializeField] private ItemWeaponSwordData weaponData;

        private ItemWeaponSwordData _currWeaponData;
        
        public IItemWeaponData ItemData => weaponData;
        IItemData IItem.ItemData => ItemData;
        
        public void Start()
        {
            _currWeaponData = Instantiate(weaponData);
        }
    }
}
