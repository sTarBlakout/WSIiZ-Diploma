using System;

namespace Gameplay.Interfaces
{
    public interface IPawnInteractable : IPawn
    {
        void PreInteract(IPawnNormal interactor, Action onPreInteract);
        void PostInteract(Action onPostInteract);
        void Interact(Action onInteract);
        IPawnInteractableData PawnData { get; }
    }
}
