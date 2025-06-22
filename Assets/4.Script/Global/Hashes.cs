using UnityEngine;

public static class AnmimatorHashes
{
      #region Animation Hash
    [Tooltip("Hash Key는 대문자로 표기")]
    [HideInInspector] public static int _MOVEANIMATION = Animator.StringToHash("MoveAnimIndex");
    [HideInInspector] public static int _MOVE = Animator.StringToHash("Move");
    [HideInInspector] public static int _MOVEFACTOR = Animator.StringToHash("MoveFactor");
    [HideInInspector] public static int _KILLMOVE = Animator.StringToHash("KillMove");
    [HideInInspector] public static int _KILLED = Animator.StringToHash("Killed");
    [HideInInspector] public static int _KILLANIMATION = Animator.StringToHash("KilledAnimIndex");
    [HideInInspector] public static int _ISPLAYER = Animator.StringToHash("IsPlayer");
    [HideInInspector] public static int _FALLDOWN = Animator.StringToHash("FallDown");
    [HideInInspector] public static int _SNIPE = Animator.StringToHash("Snipe");
   

  #endregion
}
