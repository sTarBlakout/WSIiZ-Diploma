using System.Collections;
using System.Collections.Generic;
using Gameplay.Core;
using Gameplay.Interfaces;
using UnityEngine;

public interface IInventory
{
    public bool HasAnyItems(ItemType type);
    public void EquipItem(IItem item);
    public List<(IItem, bool)> GetInventoryItems(ItemType type);
    public void AddItems(List<IItem> items);

}
