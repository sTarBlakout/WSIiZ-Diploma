using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

namespace Gameplay.Pawns
{
    public class PawnInventory : MonoBehaviour, IInventory
    {
        [SerializeField] private Transform weaponHoldPosition;

        private List<(IItem item, bool isEquipped)> items = new List<(IItem, bool)>();
            
        private IItemWeapon equippedWeapon;
        private PawnAnimator _animator;

        public void Init(PawnAnimator animator)
        {
            _animator = animator;
        }
        
        public bool HasWeapon()
        {
            return equippedWeapon != null;
        }

        public int GetWeaponDamageModifier()
        {
            return equippedWeapon.ItemData.DamageModifier;
        }

        private void AddWeapon(IItemWeapon weapon)
        {
            items.Add((weapon, false));
            if (equippedWeapon == null) EquipWeapon(weapon);
        }
        
        private void EquipWeapon(IItemWeapon weapon)
        {
            var oldTuple = items.FirstOrDefault(item => item.item == weapon);
            if (oldTuple.item == null) return;
            
            var newTuple = (oldTuple.item, true);
            items[items.IndexOf(oldTuple)] = newTuple;
            
            if (equippedWeapon != null) Destroy(equippedWeapon.ItemGameObject);
            
            equippedWeapon = Instantiate(weapon.ItemGameObject).GetComponent<IItemWeapon>();
            equippedWeapon.ItemGameObject.transform.parent = weaponHoldPosition;
            equippedWeapon.ItemGameObject.transform.localPosition = Vector3.zero;
            
            _animator.OverrideController(weapon.ItemData.OverrideController);
        }

        #region IInventory Implementation

        public List<(IItem, bool)> GetInventoryItems(ItemType type)
        {
            return items.Where(item => item.item.ItemData.ItemType == type).ToList();
        }

        public void AddItems(List<IItem> items)
        {
            foreach (var item in items)
            {
                switch (item.ItemData.ItemType)
                {
                    case ItemType.Weapon: AddWeapon((IItemWeapon) item); break;
                }
            }
        }
        
        public void EquipItem(IItem item)
        {
            switch (item.ItemData.ItemType)
            {
                case ItemType.Weapon: EquipWeapon((IItemWeapon) item); break;
            }
        }

        #endregion
    }
}
