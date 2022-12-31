using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Items.Weapons
{
    public class ItemWeaponMelee : MonoBehaviour, IItemWeapon
    {
        [SerializeField] private ItemWeaponSwordData weaponData;

        private ItemWeaponSwordData _currWeaponData;

        public GameObject ItemGameObject => gameObject;
        public IItemWeaponData ItemData => weaponData;
        IItemData IItem.ItemData => ItemData;

        public void Start()
        {
            _currWeaponData = Instantiate(weaponData);
        }
    }
}
