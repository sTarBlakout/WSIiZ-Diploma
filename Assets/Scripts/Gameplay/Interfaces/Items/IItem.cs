using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IItem
    {
        IItemData ItemData { get; }
        GameObject ItemGameObject { get; }
    }
}
