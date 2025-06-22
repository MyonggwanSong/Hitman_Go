using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [Header("Movement Attribute")]
    public float moveSpeed = 5f;
    [Header("Mesh index")]
    [SerializeField] int mesheIndex;

    [Header("Position states")]
    public Node currentNode = null;
    public Node nextNode;   // 다음 노드
    public List<Node> aiRouteNodes = new List<Node>();  // AI 경로의 노드들


    [Header("AI Route target node")]
    public Node targetNode;

    public bool isLookingPlayer; // 플레이어 쳐다보는 중

    public Transform deathZone;

    private bool isMoving = false;
    private bool isdead = false;
    private Coroutine _co = null;
    private MeshRenderer[] meshes;

    [HideInInspector] public Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();      // animation casing
        if (animator == null)
            Debug.LogWarning("CharacterControl ] Animator 없음");
        meshes = GetComponentsInChildren<MeshRenderer>();
        if (meshes == null)
            Debug.LogWarning("meshe 없음");
    }

    IEnumerator Start()
    {
       yield return new WaitUntil(()=> currentNode != null);
        SetInitializing();

        Vector3 lookDir = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
        transform.LookAt(lookDir);
        
    }
    void SetInitializing()
    {
        //TEMP
        //int rnd = Random.Range(0, currentNode.connectedNodes.Count);
        aiRouteNodes = AStarSearch.FindPath(currentNode, targetNode);
        nextNode = aiRouteNodes[1];
        //TEMP

        foreach (var m in meshes)
            m.gameObject.SetActive(false);

        meshes[mesheIndex].gameObject.SetActive(true);


    }
    public void OnTurnChanged(uint newTurn)
    {
        // 턴 넘어갔을 때 AI 동작
        if(isdead) return;
        AiMove();

    }

int _i = 2;
    void AiMove()
    {
        if (currentNode.connectedNodes == null || currentNode.connectedNodes.Count == 0)
        {
            Debug.LogWarning("No connected nodes to move.");
            return;
        }
        OnEnemyTurn();
        //TEMP
        //int rnd = Random.Range(0, currentNode.connectedNodes.Count);
        if(_i<aiRouteNodes.Count)
        {

        nextNode = aiRouteNodes[_i];
        _i ++;
        }
        else
        {
            // 다음 행동
            _i = 2;
        }
        //TEMP
    }

    void OnEnemyTurn() // AI가 직접 이동
    {
        if (nextNode != null)
        {
            _co = StartCoroutine(MoveToPosition(nextNode.transform.position));
            currentNode = nextNode;
        }
        else
        {
            Debug.Log("Cannot move: No target Node.");
        }
    }


    public bool CanSeePlayer(PlayerControl player)
    {
        return isLookingPlayer; // 간단 예시
    }

    public void Kill()
    {
        if(isdead) return;

        Debug.Log("적 처치됨!");
        StopCoroutine(_co);
        AnimateBool(AnmimatorHashes._KILLED, true, AnmimatorHashes._KILLANIMATION, 3, false);
        isdead =true;
        GameManager.I.killedEnemyNum ++;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float time = 0f;

          // 애니메이ㅣ션
        AnimateBool(AnmimatorHashes._MOVE, true, AnmimatorHashes._MOVEANIMATION, 5,true);

        while (time < 1f)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        transform.position = targetPos;
        Vector3 lookDir = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
        transform.LookAt(lookDir);
        isMoving = false;

        
    }

    #region Animation
    // 애니메이션 해시 코드를 찾아서 파라미터 움직임.
    public void Animate(int hash, float duration)
    {
        animator?.CrossFadeInFixedTime(hash, duration);
    }

    // bool로 들어오는 애니메이션
    public void AnimateBool(int hash, bool b)
    {
        animator?.SetBool(hash, b);
    }

    public void AnimateBool(int hash, bool b, int index, int count, bool isplayer)
    {
        animator.SetBool(AnmimatorHashes._ISPLAYER, isplayer);
        int rndMove = Random.Range(0, count);
        animator?.SetFloat(index, rndMove);
        animator?.SetBool(hash, b);
    }

    #endregion
}
