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
            Debug.Log("애니 들어옴");
        }
    }

    public void OnAnimationEnd(string s)
    {
        if (s.ToLower() == "move")
        {
            owner.animator.SetBool(AnmimatorHashes._MOVE, false);
            Debug.Log("애니 나감");
        }

        if (s.ToLower() == "killed")
        {
          
            owner.animator.SetBool(AnmimatorHashes._KILLED, false);
            Debug.Log("애니 나감");
        }
    }
}
