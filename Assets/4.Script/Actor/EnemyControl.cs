using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;
using Unity.VisualScripting;

public class EnemyControl : MonoBehaviour
{
    [Title("Movement Attribute")]
    [SerializeField] float moveSpeed = 5f;


    [Title("Patrol Guard")]
    [SerializeField] bool patrolEnemy = false;
    
    [Title("Standing Guard")]
    [SerializeField] public bool isStatic;

    [Title("Receive Events")]
    [SerializeField] EventDetect eventDetect;
    

    [HorizontalLine("Readonly for Debugging", color: FixedColor.Gray), HideField] public bool _l0;
    [Title("Mesh index")]
    [ReadOnly] public int mesheIndex;

    [Title("Position states")]
    [ReadOnly] public Node currentNode = null;
    [ReadOnly] public Node nextNode;   // 다음 노드

    [ReadOnly] public bool isDead = false;
    [ReadOnly] public List<Node> aiRouteNodes = new List<Node>();  // AI 경로의 노드들


    [Title("AI Route target node")]
    public Node targetNode;

    [Title("Tomb")]
    [ReadOnly] public Transform deathZone;
    [HorizontalLine(color: FixedColor.Gray), HideField] public bool _l1;

    private Coroutine _co = null;
    private Transform meshRoot;
    private MeshRenderer[] foundMeshes;
    [HideInInspector] public Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();      // animation casing
        if (animator == null)
            Debug.LogWarning("EnemyControl ] Animator 없음");

        meshRoot = NewBaseType.FindSlot(transform, "Mesh");
        if (meshRoot == null)
            Debug.LogWarning("EnemyControl ] MeshRoot 없음");

        foundMeshes = meshRoot.GetComponentsInChildren<MeshRenderer>();
        if (foundMeshes == null || foundMeshes.Length == 0)
            Debug.LogWarning("MeshRoot ] meshe 없음");

    }

    void OnEnable()
    {
        eventDetect.Register(OnEventDetect);
    }

    void OnDisable()
    {
        eventDetect.Unregister(OnEventDetect);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => foundMeshes != null);
        foreach (var m in foundMeshes)
            m.gameObject.SetActive(false);
        foundMeshes[mesheIndex].gameObject.SetActive(true);
    }


    public void SetInitial() // A* 경로 세팅
    {
        aiRouteNodes = AStarSearch.FindPath(currentNode, targetNode);

        if (aiRouteNodes.Count > 1)
        {
            nextNode = aiRouteNodes[1];
            Vector3 lookDir = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
             transform.LookAt(lookDir);
        }
        else
        {
            Debug.LogWarning("A* 경로 생성 실패");
            return;
        }
            
    }

    public void OnTurnChanged(uint newTurn)
    {
        // 턴 넘어갔을 때 AI 동작
        if (isDead) return;

        AiMove();

    }

    int _i = 2;
    void AiMove()
    {
        if (isStatic) return;
        if (currentNode.connectedNodes == null || currentNode.connectedNodes.Count == 0)
        {
            Debug.LogWarning("No connected nodes to move.");
            return;
        }
        OnEnemyTurn();
        if (_i < aiRouteNodes.Count)
        {

            nextNode = aiRouteNodes[_i];
            _i++;
        }
        else    // A*경로 마지막 도착
        {
            AnimateBool(AnmimatorHashes._INVESTIGATE, false);
            _i = 2;
            if (patrolEnemy)    // 순찰하는 적 경로 설정
            {
                Vector3 _previousDir = aiRouteNodes[aiRouteNodes.Count - 1].transform.position - aiRouteNodes[aiRouteNodes.Count - 2].transform.position;
                targetNode = AStarSearch.FindFarthestNode(currentNode, _previousDir);
                if (targetNode == null)
                    return;
                SetInitial();
            }
            else
            {
                isStatic = true;
            }
        }

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


    public void Kill()
    {
        if (isDead) return;

        Debug.Log("적 처치됨!");
        if (_co != null)
            StopCoroutine(_co);
        AnimateBool(AnmimatorHashes._KILLED, true, AnmimatorHashes._KILLANIMATION, 3, false);
        isDead = true;
        GameManager.I.killedEnemyNum++;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float time = 0f;

        // 애니메이ㅣ션
        AnimateBool(AnmimatorHashes._MOVE, true, AnmimatorHashes._MOVEANIMATION, 5, true);

        while (time < 1f)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        transform.position = targetPos;
        Vector3 lookDir = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
        transform.LookAt(lookDir);
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
    #region Event
    void OnEventDetect(EventDetect e)   // Detect Event
    {
        float distance = Vector3.Distance(currentNode.transform.position, e.occuredNode.transform.position);
        if (distance < e.DetectRadius)
        {
            targetNode = e.occuredNode;

            AnimateBool(AnmimatorHashes._INVESTIGATE, true);
            isStatic = false;
            SetInitial();
        }
    }


    #endregion
}
