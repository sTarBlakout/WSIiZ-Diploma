namespace Gameplay.Interfaces
{
    public interface IItemWeaponData : IItemData
    {
        int DamageModifier { get; }
        int RangeModifier { get; }
    }
}
