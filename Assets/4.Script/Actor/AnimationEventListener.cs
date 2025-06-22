using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{

     private PlayerControl owner;

       private void Awake()
    {
        owner = GetComponentInParent<PlayerControl>();
        if (owner == null)
            Debug.LogWarning("PlayerControl ] Animator 없음");
    }


    public void OnAnimationStart(string s)
    {
        if(s.ToLower() == "move")
        {
            // 애니메이션 들어올 때 이벤트
        }
    }

    public void OnAnimationEnd(string s)
    {
        if (s.ToLower() == "move")
        {
            owner.animator.SetBool(AnmimatorHashes._MOVE, false);
        }

        if (s.ToLower() == "killed")
        {
            owner.animator.SetBool(AnmimatorHashes._KILLED, false);
        }
    }
}
