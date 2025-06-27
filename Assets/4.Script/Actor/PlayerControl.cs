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

    [HorizontalLine("Readonly for Debugging", color: FixedColor.Gray), HideField] public bool _l0;
    [Title("Targets")]


    [Title("Now You Are Here")]
    [ReadOnly] public Node currentNode; // 현재 위치한 노드
    [Title("You have this Items")]
    [ReadOnly] public Item hasItem;    // 가지고 있는 아이템


    [HorizontalLine(color: FixedColor.Gray), HideField] public bool _l1;

    [HideInInspector] public Animator animator;
    private EnemyControl[] enemies;
    private List<GameObject> arrowPool = new List<GameObject>();
    private Vector3 dragStartPos;   // 마우스 클릭위치
    private Vector3 dragEndPos;     // 마우스 땐 위치
    private bool isDragging = false;    // 마우스 클릭 중

    private bool isMoving = false;

    void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();  // animation casing
        if (animator == null)
            Debug.LogWarning("CharacterControl ] Animator 없음");
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.I.isGameStart);

        enemies = FindObjectsOfType<EnemyControl>();
        GameManager.I.enemyNum = enemies.Length;
    }


    void Update()
    {
        if (GameManager.I.isGameStart == false || GameManager.I.isGameover == true) return; //  시작전, 게임오버면 return


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // mouse Point to Ray
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 플래이어 클릭 시
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                if (Input.GetMouseButtonDown(0) && !isMoving && hasItem == null)
                {
                    OnMouseButtenDown();
                }
            }
            if (hit.collider != null && hit.collider.tag == "Indicator_Targetable")
            {
                if (Input.GetMouseButtonDown(0) && !isMoving && hasItem != null)
                {
                    Collider[] cols = Physics.OverlapSphere(hit.collider.transform.position, 0.5f); // 인디케이터와 0.5거리의 노드들을 찾아서 캐싱
                    List<Node> detectedNodes = new List<Node>();
                    foreach (Collider c in cols)
                    {
                        if (!c.CompareTag("Node")) continue;
                        Node targetnode = c.GetComponentInParent<Node>();
                        detectedNodes.Add(targetnode);

                    }

                    Throwing(detectedNodes[0]);
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
            currentNode = targetNode;
            CheckInteraction();
            StartCoroutine(MoveToPosition(targetNode.transform.position));
        }

        else
            Debug.Log("Can not move because no target Node.");
    }

    void CheckInteraction() // 상효작용 체크
    {
        // 아이템
        if (currentNode.hasItem)
        {
            FindEnemyOnNode(currentNode);
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
        FindEnemyOnNode(currentNode);


        // 골 확인
        if (currentNode.isGoalNode)
        {
            GameManager.I.isGameStart = false;
           StartCoroutine(ClearLevel());
        }
    }


    void FindEnemyOnNode(Node node) 
    {

        foreach (var enemy in enemies)
        {
            Debug.Log($"Checking enemy: {enemy.name}, currentNode: {enemy.currentNode?.name}, target node: {node.name}");
            if (enemy.currentNode == node && enemy.isDead == false)
            {


                if (enemy.isTarget) // Target
                {
                    Time.timeScale = 0.3f; //  Slow Motion
                    Time.fixedDeltaTime = 0.02f * Time.timeScale; //  Slow Motion

                    enemy.Kill();

                    GameManager.I.killedTarget = true;
                    GameManager.I.isGameStart = false;
                    StartCoroutine(ClearLevel());
                }
                else    // Normal
                {
                    enemy.Kill();
                }

            }
            if (enemy.nextNode == node && enemy.isDead == false)
            {
                enemy.isStatic = false;
                Die();
            }
           
        }
    }


    #region Indicator
    public void InitializeArrowIndicators()
    {
        if (moveSign == null)
        {
            Debug.Log("moveSign Missing");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject arrow = Instantiate(moveSign, transform.position, Quaternion.identity, transform);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }

        UpdateArrowIndicators();
    }
    public void UpdateArrowIndicators()
    {
        Vector3 lookDir = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);    // 카메라를 바라봐~
        transform.LookAt(lookDir);
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
    #endregion
    #region Movement
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
        if (currentNode == null)
        {
            Debug.Log("currentNode 없음, 화살표 갱신 불가");

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

#region Activate
    void CollectItem()  // 아이템 획득 코드
    {
         if (currentNode != null)
            foreach (var arrow in arrowPool)
            {
                arrow.SetActive(false);
            }
        //애니매이션, mesh 교체
    }

    public void Throwing(Node target)
    {
        if (hasItem == null) return;

        hasItem.targetNode = target;
        StartCoroutine(hasItem.ParabolaMoveCoroutine());
        hasItem = null;

        if (currentNode != null)
            UpdateArrowIndicators();
    }
    void Hide()     // 은신 처리
    {

    }
    public void Die()  // 사망처리
    {
        AnimateBool(AnmimatorHashes._KILLED, true, AnmimatorHashes._KILLANIMATION, 3, true);

        if (arrowPool != null)  // 인디케이터 끄기
            foreach (var arrow in arrowPool)
            {
                arrow.SetActive(false);
            }

        GameManager.I.isGameover = true;
        Debug.Log("Game Over!!");

        StartCoroutine(DelayRestart(2f));
    }
    IEnumerator DelayRestart(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        UIManager.I.OnSceneReload();
    }
    IEnumerator ClearLevel()   // 클리어 처리
    {
        yield return new WaitUntil(() => GameManager.I.isGameStart == false);


        AchievementManager.I.SetLevelCleared(GameManager.I.currentLevel);   // Clear Level Save
        AchievementManager.I.EvaluateAchievements();    // Clear Achievement save

        yield return new WaitForSeconds(1f);

        if (GameManager.I.killedTarget) // Rese TimeScale
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        UIManager.I.OnClearPopup();
        for (int i = 0; i < UIManager.I.stamps.Count; i++) // 업적에 해당하는 도장 키기
        {
            if (AchievementManager.I.IsLevelAchievment(GameManager.I.currentLevel, i + 1))
            {

                UIManager.I.stamps[i].SetActive(true);
            }
        }
        Debug.Log("Game Clear!!");
    }

    #endregion


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
