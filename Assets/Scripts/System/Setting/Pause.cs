using UnityEngine;

public static class Pause
{
    public static bool IsPause
    {
        get => Time.timeScale == 0f;
        set => Time.timeScale = value ? 0f : 1f;
    }
}
