namespace StateMachine.Core
{
    public enum ErrorCodes
    {
        AlreadyPresentState = 1,
        UnknownState,
        InvalidArc,
        AlreadyPresentArc,
        UnknownArc,
        InvalidTransition,
        //PostedStateAreadySet,
        AlreadyTransiting
    }
}
