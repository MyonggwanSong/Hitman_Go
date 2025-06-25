using UnityEngine;




public class AchievementManager : BehaviourSingleton<AchievementManager>
{
    protected override bool IsDontdestroy() => false;

    public LevelAchievementData levelData; // 인스펙터에서 data 연결

    void Start()
    {
        GameManager.I.currentLevel = levelData.levelNumber;
    }
    public void EvaluateAchievements()
    {
        for (int i = 0; i < levelData.achievementConditions.Length; i++)
        {
            if (levelData.achievementConditions[i].IsConditionMet())
            {
                SetLevelAchievment(levelData.levelNumber, i + 1); // 업적 인덱스 1부터
                Debug.Log($"클리어! 업적{i + 1} 달성");
            }
        }
    }



    #region Playerprfs
    public bool IsLevelCleared(int Level)
    {
        return PlayerPrefs.GetInt($"Level_{Level}_Clear", 0) == 1;
    }
    public bool IsLevelAchievment(int Level, int Achievment)
    {
        return PlayerPrefs.GetInt($"Level_{Level}_Achievment_{Achievment}", 0) == 1;
    }


    public void SetLevelCleared(int Level)
    {
        PlayerPrefs.SetInt($"Level_{Level}_Clear", 1);
        PlayerPrefs.Save();
    }
    public void SetLevelAchievment(int Level, int Achievment)
    {
        PlayerPrefs.SetInt($"Level_{Level}_Achievment_{Achievment}", 1);
        PlayerPrefs.Save();
    }
    #endregion
}
