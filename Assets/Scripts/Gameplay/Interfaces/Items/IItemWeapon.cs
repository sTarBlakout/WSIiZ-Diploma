namespace Gameplay.Interfaces
{
    public interface IItemWeapon : IItem
    {
        new IItemWeaponData ItemData { get; }
    }
}
