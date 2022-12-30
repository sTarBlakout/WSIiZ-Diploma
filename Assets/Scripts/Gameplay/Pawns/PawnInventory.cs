using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnInventory : MonoBehaviour
    {
        [SerializeField] private Transform weaponHoldPosition;

        private List<(IItem item, bool isEquipped)> items = new List<(IItem, bool)>();
            
        private IItemWeapon equippedWeapon;

        public void AddItems(List<IItem> items)
        {
            var item = items[0];
            
            switch (item.ItemData.ItemType)
            {
                case ItemType.Weapon: AddWeapon((IItemWeapon) item); break;
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

        public List<(IItem, bool)> GetItems(ItemType type)
        {
            return items;
        }

        private void AddWeapon(IItemWeapon weapon)
        {
            items.Add((weapon, false));
            if (equippedWeapon == null) EquipWeapon(weapon);
        }
        
        private void EquipWeapon(IItemWeapon weapon)
        {
            var oldTuple = items.First(item => item.item == weapon);
            var newTuple = (oldTuple.item, true);
            items[items.IndexOf(oldTuple)] = newTuple;
            
            if (equippedWeapon != null) Destroy(equippedWeapon.ItemGameObject);
            
            equippedWeapon = Instantiate(weapon.ItemGameObject).GetComponent<IItemWeapon>();
            equippedWeapon.ItemGameObject.transform.parent = weaponHoldPosition;
            equippedWeapon.ItemGameObject.transform.localPosition = Vector3.zero;
        }
    }
}
