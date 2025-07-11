using UnityEngine;
public class GameManager : BehaviourSingleton<GameManager>
{
    protected override bool IsDontdestroy() => true;


    #region Achievement
    public int currentLevel; // 단계
    public uint turn;   // 진행 턴
    public bool getTargetItem; // 목표 아이템
    public bool killedTarget;   // 목표 타겟
    public int enemyNum;    // 총 적 수
    public int killedEnemyNum;  // 죽인 적 수
    #endregion
    #region Game State
    public bool isGameover = false; // 게임오버
    public float startDelay = 3f;   // 시작 딜레이
    public bool isGameStart = false;

    #endregion
    void Start()
    {
        SetInitialIngame();
    }
    public void SetInitialIngame()
    {
        turn = 0;
        getTargetItem = false;
        killedTarget = false;
        enemyNum = 0;
        killedEnemyNum = 0;
        isGameover = false;
        isGameStart = false;
    }


   


}
