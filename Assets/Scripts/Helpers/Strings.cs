public class Strings  {
    // TODO - This should be reorganized..  I have no idea where any of these
    // are being used or what they're for.
    public static string MOD = "Mod";
    public static string UP = "Up"; // REMOVE?
    public static string DOWN = "Down"; // REMOVE?
    public static string LEFT = "Left"; // REMOVE?
    public static string RIGHT = "Right"; // REMOVE?
    public static string PREPARETOSWAP = "PrepareToSwap"; // REMOVE
    public static string PREPARETOPICKUPORDROP = "PrepareToPickUpOrDrop"; // REMOVE
    public static string AIMX = "AimX"; // REMOVE
    public static string AIMY = "AimY"; // REMOVE
    public static string MOVEX = "MoveX"; // REMOVE
    public static string MOVEY = "MoveY"; // REMOVE
    public static string ENEMY = "Enemy";
    public static string PLAYER = "Player";
    public static string CODEXENTRY = "CodexEntry";
    public static string LEFTTRIGGER = "LeftTrigger"; // REMOVE
    public static string RIGHTTRIGGER = "RightTrigger"; // REMOVE
    public static string UNLOCKABLE = "Unlockable";
    public static string ANIMATIONSTATE = "AnimationState";
    public static string DPAD_X = "D-PadX"; // REMOVE
    public static string DPAD_Y = "D-PadY"; // REMOVE

    public static class Input
    {
        public static class Axes
        {
            public const string MOVEMENT = "MOVEMENT AXES";
            public const string LOOK = "LOOK AXES";
        }

        public static class Actions
        {
            public const string SWAP_MODE = "SWAP MODE";
            public const string DROP_MODE = "DROP MODE";
            public const string ACTION_LEGS = "LEGS";
            public const string ACTION_ARM_LEFT = "ARM LEFT";
            public const string ACTION_ARM_RIGHT = "ARM RIGHT";
            public const string ACTION_HEAD = "HEAD";
            public const string NAV_UP = "UP";
            public const string NAV_DOWN = "DOWN";
            public const string NAV_LEFT = "LEFT";
            public const string NAV_RIGHT = "RIGHT";
        }
    }
}
