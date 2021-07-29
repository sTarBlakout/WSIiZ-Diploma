using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IInteractable
    {
        bool IsInteractable();
        Vector3 Position { get; }
    }
}
