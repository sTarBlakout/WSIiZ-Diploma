namespace Gameplay.Core
{
    public enum PawnRelation
    {
        Friend,
        Enemy,
        Consumable
    }
    
    public enum OrderType
    {
        None,
        Attack,
        Move
    }
    
    public enum TileParticleType
    {
        ReachableTile,
        ReachableEnemy
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
