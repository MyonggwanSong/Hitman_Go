using UnityEngine;

public class EnemyAnimationEventListener : MonoBehaviour
{

     private EnemyControl owner;

       private void Awake()
    {
        owner = GetComponentInParent<EnemyControl>();
        if (owner == null)
            Debug.LogWarning("EnemyControl ] Animator 없음");
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

            owner.transform.position = owner.deathZone.position + new Vector3(-GameManager.I.killedEnemyNum*0.8f,0f,0f); // 죽었을때 떨어지는 위치
            owner.AnimateBool(AnmimatorHashes._FALLDOWN, true, AnmimatorHashes._KILLANIMATION, 3, false);
            Debug.Log("애니 나감");
        }
    }
}
