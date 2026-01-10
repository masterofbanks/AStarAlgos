using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int CurrentLevel = 0;
    public LevelManager LevelManagerScript;
    public static LevelLoader LevelLoadInstance { get; private set; }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (LevelLoadInstance == null)
        {
            LevelLoadInstance = this;
        }

        else
        {
            Destroy(LevelLoadInstance.gameObject);
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
