public class GameManager : BehaviourSingleton<GameManager>
{
    protected override bool IsDontdestroy() => true;

    public float startDelay = 1f;

    #region Achievement

    public uint turn;
    public bool getTargetItem;
    public bool killedTarget;
    public int enemyNum;
    public int killedEnemyNum;

    #endregion
}
