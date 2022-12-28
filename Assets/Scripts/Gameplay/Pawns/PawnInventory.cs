using System;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnInventory : MonoBehaviour
    {
        [SerializeField] private Transform weaponHoldPosition;
        
        private IItemWeapon equippedWeapon;

        public void AddItems(List<IItem> items)
        {
            var item = items[0];
            
            switch (item.ItemData.ItemType)
            {
                case ItemType.Weapon: EquipWeapon((IItemWeapon) item); break;
            }
        }

        public bool HasWeapon()
        {
            return equippedWeapon != null;
        }

        public int GetWeaponDamageModifier()
        {
            return equippedWeapon.ItemData.DamageModifier;
        }

        private void EquipWeapon(IItemWeapon weapon)
        {
            if (equippedWeapon != null) Destroy(equippedWeapon.ItemGameObject);
            
            equippedWeapon = Instantiate(weapon.ItemGameObject).GetComponent<IItemWeapon>();
            equippedWeapon.ItemGameObject.transform.parent = weaponHoldPosition;
            equippedWeapon.ItemGameObject.transform.localPosition = Vector3.zero;
        }
    }
}
