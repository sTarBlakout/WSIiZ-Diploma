namespace Gameplay.Interfaces
{
    public interface IPawnNormal : IPawn
    {
        bool IsAlive { get; }
        IDamageable Damageable { get; }
    }
}
