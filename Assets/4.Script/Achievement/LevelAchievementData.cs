using UnityEngine;

[CreateAssetMenu(fileName = "LevelAchievementData", menuName = "Achievement/LevelData")]
public class LevelAchievementData : ScriptableObject
{
    public int levelNumber;
    public AchievementCondition[] achievementConditions = new AchievementCondition[3];
}
