using UnityEngine;

namespace Gameplay.Interfaces
{
    public interface IPawnData
    {
        Vector3 Position { get; }
        int TeamId { get; }
        int DamageValue { get; }
        void ModifyLevelBy(int value);
    }
}
