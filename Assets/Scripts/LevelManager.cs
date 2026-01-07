using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "LevelManagerAsset", menuName = "Levels/LevelManagement")]
public class LevelManager : ScriptableObject
{
    [System.Serializable]
    public class LevelStats
    {
        public GameObject Fruit;
        public float EnemySpeed;
        public float TimeInFrightenedMode;
    }

    public List<LevelStats> Levels;
}
