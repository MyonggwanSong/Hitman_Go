using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;


public class EnemyControl : MonoBehaviour
{
    [Title("Movement Attribute")]
    [SerializeField] float moveSpeed = 5f;

    [Title("State PopupLabel prefab")]
    [SerializeField] GameObject detectPopupLabel;
    [SerializeField] private float labelOffsetY = 2f;  // 머리 위 y 위치


    [Title("Patrol Guard")]
    [SerializeField] bool patrolEnemy = false;

    [Title("Standing Guard")]
    [SerializeField] public bool isStatic;

    [Title("Target")]
    [SerializeField] public bool isTarget;

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

        meshRoot = NewBaseType.FindSlot(transform, "Mesh"); // 가까운데에서 찾기위한 슬롯 캐싱
        if (meshRoot == null)
            Debug.LogWarning("EnemyControl ] MeshRoot 없음");



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
        foundMeshes = meshRoot.GetComponentsInChildren<MeshRenderer>(); //  Mesh Casing
        if (foundMeshes == null || foundMeshes.Length == 0)
            Debug.LogWarning("MeshRoot ] meshe 없음");

        yield return new WaitUntil(() => foundMeshes != null);
        foreach (var m in foundMeshes)
            m.gameObject.SetActive(false);
        foundMeshes[mesheIndex].gameObject.SetActive(true);

    }

    private void Update()
    {
        if (detectPopupLabel != null && detectPopupLabel.activeSelf)
        {
            Vector3 basePos = transform.position + Vector3.up * labelOffsetY;
            detectPopupLabel.transform.position = basePos;
            detectPopupLabel.transform.LookAt(Camera.main.transform.position);
        }
    }
    public void SetInitial() // A* 경로 세팅
    {
        if (targetNode == null) return;

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



        if (!ValidateAIRoute(aiRouteNodes))
        {
            Debug.LogError("[EnemyControl] Invalid AI Route: contains disconnected nodes!");
        }



        OnEnemyTurn();
        if (_i < aiRouteNodes.Count)
        {
            nextNode = aiRouteNodes[_i];
            _i++;
        }
        else    // AI route finish
        {
            AnimateBool(AnmimatorHashes._INVESTIGATE, false);
            _i = 2;




            if (patrolEnemy)    // Patrol 일 때
            {
                if (detectPopupLabel != null && detectPopupLabel.activeSelf)
                    detectPopupLabel.SetActive(false);


                {
                    Vector3 _previousDir = (aiRouteNodes[^1].transform.position - aiRouteNodes[^2].transform.position).normalized;      //마지막과 이전 노드의 방향 
                    targetNode = AStarSearch.FindFarthestNode(currentNode, _previousDir); // 가장 먼 노드를 찾아 타겟노드로 넣기

                    if (targetNode != null)
                        SetInitial();
                }
               

            }
            else   // Standing 일 때
            {
                if (detectPopupLabel != null && detectPopupLabel.activeSelf)
                    detectPopupLabel.SetActive(false);

               
                    Vector3 _previousDir = (aiRouteNodes[^1].transform.position - aiRouteNodes[^2].transform.position).normalized;
                    targetNode = AStarSearch.FindForwardNode(currentNode, _previousDir);

                    if (targetNode != null)
                        SetInitial();
                
                isStatic = true;
            }
        }
    }

    public bool ValidateAIRoute(List<Node> route)
{
    for (int i = 0; i < route.Count - 1; i++)
    {
        Node from = route[i];
        Node to = route[i + 1];

        if (!from.connectedNodes.Contains(to))
        {
            Debug.LogWarning($"[ValidateAIRoute] {from.name} is not connected to {to.name}");
            return false;
        }
    }
    return true;
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

    #region Acvtivate
    public void Kill()
    {
        if (isDead) return;

        Debug.Log("적 처치됨!");
        if (_co != null)
            StopCoroutine(_co);
        AnimateBool(AnmimatorHashes._INVESTIGATE, false);   // 
        
        detectPopupLabel.SetActive(false);

        AnimateBool(AnmimatorHashes._KILLED, true, AnmimatorHashes._KILLANIMATION, 3, isTarget);
        isDead = true;
        GameManager.I.killedEnemyNum++;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float time = 0f;

        // 애니메이션
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
    #endregion
    private IEnumerator DetectLabelAnimation()
    {
        detectPopupLabel.SetActive(true);

        float startY = labelOffsetY;
        float endY = startY - 0.3f;

        float time = 0f;
        float duration = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            labelOffsetY = Mathf.Lerp(startY, endY, t);

            yield return null;
        }

        labelOffsetY = endY;
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
        if (isDead == true) return;
        float distance = Vector3.Distance(currentNode.transform.position, e.occuredNode.transform.position);
        if (distance < e.DetectRadius)
        {
            targetNode = e.occuredNode;

            AnimateBool(AnmimatorHashes._INVESTIGATE, true);
            isStatic = false;
            SetInitial();
            StartCoroutine(DetectLabelAnimation());
        }
    }


    #endregion
}
