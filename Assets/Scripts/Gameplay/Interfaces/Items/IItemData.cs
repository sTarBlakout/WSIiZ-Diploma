using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IItemData
    {
        int ItemId { get; }
        string ItemName { get; }
        Sprite ItemIcon { get; }
        ItemType ItemType { get; }
        AnimatorOverrideController OverrideController { get; }
    }
}
