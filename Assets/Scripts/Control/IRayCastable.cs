namespace Control
{
    public interface IRayCastable
    {
        CursorState GetCursorState();
        bool HandleRaycast(PlayerController callingController);
    }
}
