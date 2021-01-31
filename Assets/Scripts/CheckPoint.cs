using UnityEngine;

public static class CheckPoint
{
    private static string highscoreKey = "Highscore";
    public static float progress;
    
    public static void Save(float z)
    {
        progress = z;
        PlayerPrefs.SetFloat(highscoreKey, z);
    }

    public static void Load()
    {
        progress = PlayerPrefs.GetFloat(highscoreKey);
    }
}
