namespace Gameplay.Core
{
    public enum PawnRelation
    {
        Friend,
        Enemy,
        Interactable,
        Unknown
    }

    public enum ItemType
    {
        Weapon
    }
    
    public enum OrderType
    {
        None,
        Attack,
        Move,
        Interact
    }
    
    public enum TileParticleType
    {
        ReachableTile,
        ReachableEnemy,
        ReachableInteractable
    }
    
    public enum OrderResult
    {
        NotDetermined,
        Succes,
        HalfSucces,
        Fail
    }

    public enum OrderFailReason
    {
        None,
        TooFar,
        BlockedArea,
        NotAnEnemy,
        Dead
    }

    public enum AnimMovement
    {
        RotateLeft,
        RotateRight,
        Walk
    }
}
