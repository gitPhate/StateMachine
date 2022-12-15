namespace StateMachine.Core
{
    public enum ErrorCodes
    {
        StateAlreadyAdded = 1,
        StateNotFound,
        InvalidArc,
        ArcAlreadyAdded,
        ArcNotFound,
        AlreadyTransiting
    }
}
