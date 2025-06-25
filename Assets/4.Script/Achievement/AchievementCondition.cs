using UnityEngine;

 public enum AchievementConditionType
{
    ClearInTurns,
    KillAllEnemies,
    NoKill,
    KillTarget,
    GetItem
}
[System.Serializable]
public class AchievementCondition
{
    public AchievementConditionType conditionType;
     [Tooltip("Clear In turns 조건일 때만 사용")]
    public int turnLimit;
    public bool IsConditionMet()
    {
        var gm = GameManager.I;
        switch (conditionType)
        {
            case AchievementConditionType.ClearInTurns:
                return gm.turn <= turnLimit;
            case AchievementConditionType.KillAllEnemies:
                return gm.killedEnemyNum == gm.enemyNum;
            case AchievementConditionType.NoKill:
                return gm.killedEnemyNum == 0;
            case AchievementConditionType.KillTarget:
                return gm.killedTarget;
            case AchievementConditionType.GetItem:
                return gm.getTargetItem;
            default:
                return false;
        }
    }
}