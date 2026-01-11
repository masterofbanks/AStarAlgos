using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int CurrentLevel;
    public LevelManager LevelManagerScript;
    public static LevelLoader LevelLoadInstance { get; private set; }
    private void Awake()
    {

        if (LevelLoadInstance == null)
        {
            LevelLoadInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public LevelManager.LevelStats GetCurrentLevelStats()
    {
        return LevelManagerScript.Levels[CurrentLevel];
    }

    public void IncramentCurrentLevel()
    {
        if(CurrentLevel < LevelManagerScript.Levels.Count - 1)
        {
            CurrentLevel++;
        }

        else
        {
            CurrentLevel = LevelManagerScript.Levels.Count - 1;
        }

    }
}
