namespace Gameplay.Core
{
    public enum Order
    {
        None,
        Move,
        Attack
    }

    public enum OrderResult
    {
        NotDetermined,
        Succes,
        Fail
    }

    public enum OrderFailReason
    {
        None,
        TooFar,
        BlockedArea,
        ExceededLimit,
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
