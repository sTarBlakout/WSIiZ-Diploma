namespace Gameplay.Core
{
    public enum OrderType
    {
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
        NotInteractable
    }

    public enum AnimMovement
    {
        RotateLeft,
        RotateRight,
        Walk
    }
}
