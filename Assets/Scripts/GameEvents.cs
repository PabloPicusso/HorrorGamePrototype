using System;

public static class GameEvents
{
    // true = ON, false = OFF
    public static event Action<bool> LighterToggled;

    public static void ToggleLighter(bool on) => LighterToggled?.Invoke(on);
}
