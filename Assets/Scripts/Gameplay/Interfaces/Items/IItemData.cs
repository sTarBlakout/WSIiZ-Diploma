using Gameplay.Core;

namespace Gameplay.Interfaces
{
    public interface IItemData
    {
        string ItemName { get; }
        ItemType ItemType { get; }
    }
}
