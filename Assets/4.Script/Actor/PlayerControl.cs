using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector] public Camera mainCamera;
    [Title("Movement Attribute")]
    public float moveSpeed = 5f;

    [Title("MoveSign to Connected Node")]
    [SerializeField] private GameObject moveSign;        // 움직일 수 있는 곳 화살표 프리팹

    [Title("Targets")]
    [ReadOnly] public EnemyControl[] allEnemies;
    // public Item[] allItems;

    [Title("Now You Are Here")]
    [ReadOnly] public Node currentNode; // 현재 위치한 노드

    [HideInInspector] public Animator animator;

    private List<GameObject> arrowPool = new List<GameObject>();
    private Vector3 dragStartPos;   // 마우스 클릭위치
    private Vector3 dragEndPos;     // 마우스 땐 위치
    private bool isDragging = false;    // 마우스 클릭 중
    private Node targetNode; // 움직일 다음 노드

    private bool isMoving = false;

    void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();  // animation casing
        if (animator == null)
            Debug.LogWarning("CharacterControl ] Animator 없음");
    }

    void Start()
    {
        // 움직일 수 있는 곳 표시용 화살표 소환
        if (moveSign == null)
        {
            Debug.Log("moveSign Missing");
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject arrow = Instantiate(moveSign, transform.position, Quaternion.identity, transform);
            arrow.SetActive(false); // 비활성화 상태로 시작
            arrowPool.Add(arrow);
        }
        UpdateArrowIndicators();
    }

    void Update()
    {
        if (GameManager.I.isGameStart == false || GameManager.I.isGameover == true) return; //  시작전, 게임오버면 return

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // mouse Point to Ray
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 플래이어 클릭 시
        {
            if (hit.collider != null && hit.collider.tag == "Player")
            {
                if (Input.GetMouseButtonDown(0) && !isMoving)
                {
                    OnMouseButtenDown();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging && !isMoving)
        {
            OnMouseButtenUp();
        }
    }
#region  Input
    void OnMouseButtenDown()
    {
        transform.position += Vector3.up * 0.2f; // Mouse Clicked feedback

        dragStartPos = Input.mousePosition;
        isDragging = true;
    }

    void OnMouseButtenUp()
    {
        transform.position -= Vector3.up * 0.2f; // Mouse Clicked feedback

        dragEndPos = Input.mousePosition;

        // 화면 좌표 >> Ray >> 월드 평면과의 교차점으로 변환
        Ray startRay = mainCamera.ScreenPointToRay(dragStartPos);
        Ray endRay = mainCamera.ScreenPointToRay(dragEndPos);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // y=0 기준 평면
        float startDistance, endDistance;

        if (groundPlane.Raycast(startRay, out startDistance) && groundPlane.Raycast(endRay, out endDistance))
        {
            Vector3 worldStart = startRay.GetPoint(startDistance);
            Vector3 worldEnd = endRay.GetPoint(endDistance);
            Vector3 dragVector = worldEnd - worldStart;


            if (dragVector.magnitude > 0.5f) // 일정 거리 이상일 때만 인정
            {
                MoveTo(MoveToDirection(dragVector.normalized));  // 다음 노드 확인 후 움직임
            }
        }
        else
        {
            Debug.LogWarning("No Ray in groundPlane");
        }

        isDragging = false;
    }
#endregion

    void MoveTo(Node targetNode) 
    {
        if (targetNode != null)
        {
            StartCoroutine(MoveToPosition(targetNode.transform.position));
            currentNode = targetNode;
            CheckInteraction();
        }

        else
            Debug.Log("Can not move because no target Node.");
    }

    void CheckInteraction() // 상효작용 체크
    {
        // 아이템
        if (currentNode.hasItem)
        {
            CollectItem();
            return;
        }

        // 은신
        if (currentNode.hasBush)
        {
            Hide();
            return;
        }

        // 적 유무 검사
        bool _isKilledZone;
        EnemyControl enemy = FindEnemyOnNode(currentNode, out _isKilledZone);
        if (enemy != null)
        {
            if (_isKilledZone == false) // 적을 죽일 수 있는 노드
                enemy.Kill();
            else                    // 적이 날 죽일 수 있는 노드
                Die();
        }

        // 골 확인
        if (currentNode.isGoalNode)
            ClearStage();
    }


    EnemyControl FindEnemyOnNode(Node node, out bool isKilldZone)  // boo
    {
        EnemyControl[] enemies = FindObjectsOfType<EnemyControl>();
        foreach (var enemy in enemies)
        {
            if (enemy.currentNode == node)
            {
                isKilldZone = false;
                return enemy;
            }
            if (enemy.nextNode == node)
            {
                isKilldZone = true;
                return enemy;
            }
        }
        isKilldZone = false;
        return null;
    }

    #region Movement
    public void UpdateArrowIndicators()
    {
        // 연결 노드 방향으로 화살표 세팅
        int arrowIndex = 0;

        foreach (Node node in currentNode.connectedNodes)
        {
            if (arrowIndex >= arrowPool.Count) break;

            Vector3 dirToNode = (node.transform.position - currentNode.transform.position).normalized;
            Vector3 spawnPos = transform.position;

            GameObject arrow = arrowPool[arrowIndex];
            arrow.transform.position = spawnPos;
            arrow.transform.rotation = Quaternion.LookRotation(dirToNode, Vector3.up);
            arrow.SetActive(true);

            arrowIndex++;
        }
    }

    Node MoveToDirection(Vector3 dragDirection) // 카메라 기준으로 동서남북 설정
    {
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3[] directions = new Vector3[4];
        directions[0] = camForward;     // North
        directions[1] = -camRight;      // West
        directions[2] = -camForward;    // South
        directions[3] = camRight;       // East

        float maxDot = -Mathf.Infinity;
        int bestIndex = 0;

        for (int i = 0; i < directions.Length; i++)
        {
            float dot = Vector3.Dot(dragDirection, directions[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestIndex = i;
            }
        }

        Vector3 targetDirection = directions[bestIndex];

        Node targetNode = GetConnectedNodeInDirection(targetDirection);
        return targetNode;
    }

    Node GetConnectedNodeInDirection(Vector3 direction)
    {
        if (currentNode == null)
        {
            Debug.LogError("currentNode is Notiong");
            return null;
        }

        Node closestNode = null;
        float maxDot = 0.7f;

        foreach (Node node in currentNode.connectedNodes)
        {
            Vector3 dirToNode = (node.transform.position - currentNode.transform.position).normalized;
            float dot = Vector3.Dot(direction, dirToNode);

            if (dot > maxDot)
            {
                maxDot = dot;
                closestNode = node;
            }
        }

        return closestNode;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        // 기존 화살표 꺼주기
        if (currentNode == null)
        {
            Debug.LogWarning("currentNode 없음 - 화살표 갱신 취소");

        }
        else
        {
            foreach (var arrow in arrowPool)
            {
                arrow.SetActive(false);
            }
        }

        //  움직이는 중
        isMoving = true;
        Vector3 startPos = transform.position;
        float time = 0f;
        // 애니메이ㅣ션
        AnimateBool(AnmimatorHashes._MOVE, true, AnmimatorHashes._MOVEANIMATION, 5, true);

        // 움직임
        while (time < 1f)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }
        // 도착
        transform.position = targetPos;
        // 화살표 업데이트
        UpdateArrowIndicators();

        TurnManager.I.currentTurn++;
        GameManager.I.turn = TurnManager.I.currentTurn;
        Debug.Log($"turn : {GameManager.I.turn}" + $"\nCurrent Node : {currentNode.name}");

        isMoving = false;
    }
    #endregion


    void CollectItem()
    {
        /* 아이템 획득 코드 */
    }
    void Hide()
    {
        /* 은신 처리 */
    }
    void Die() // 사망처리
    {
        
        AnimateBool(AnmimatorHashes._KILLED, true, AnmimatorHashes._KILLANIMATION, 3, true);
        GameManager.I.isGameover = true;
    }
    void ClearStage()
    {
        /* 클리어 처리 */
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
