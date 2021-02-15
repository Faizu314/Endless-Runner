using UnityEngine;

public static class CheckPoint
{
    private static string highscoreKey = "Highscore";
    private static string biomeKey = "Biome";
    public static float progress;
    public static int currentBiome;
    
    public static void Save(float z, int currentBiome)
    {
        progress = z;
        PlayerPrefs.SetFloat(highscoreKey, z);
        CheckPoint.currentBiome = currentBiome;
        PlayerPrefs.SetInt(biomeKey, currentBiome);
    }

    public static void Load()
    {
        progress = PlayerPrefs.GetFloat(highscoreKey);
        currentBiome = PlayerPrefs.GetInt(biomeKey);
    }
}
