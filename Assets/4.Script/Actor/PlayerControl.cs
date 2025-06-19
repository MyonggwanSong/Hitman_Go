using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [HideInInspector] public Camera mainCamera;
    [Header("Movement")]
    public float moveDistance = 5f;
    public float moveSpeed = 5f;

    [Header("MoveSign to Connected Node")]
    public GameObject moveSign;        // 움직일 수 있는 곳 화살표
    private List<GameObject> arrowPool = new List<GameObject>();
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;

    [Header("(Debug)Now You Are Here")]

    public Node currentNode; // 현재 위치한 노드
    bool isMoving = false;
    void Awake()
    {
        mainCamera = Camera.main;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))// 캐릭터 마우스 클릭
        {
            if (hit.collider != null && hit.collider.tag == "Player")
            {
                if (Input.GetMouseButtonDown(0) && !isMoving)
                {
                    Debug.Log(hit.collider);
                    transform.position += Vector3.up * 0.2f; // Mouse Clicked feedback

                    dragStartPos = Input.mousePosition;
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging && !isMoving)
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

                // Debug.Log($"DragVector (World) : {dragVector}");

                if (dragVector.magnitude > 0.5f) // 일정 거리 이상일 때만 인정
                {
                    MoveToDirection(dragVector.normalized);
                }
            }
            else
            {
                Debug.LogWarning("No Ray in groundPlane");
            }

            isDragging = false;
        }
    }



    #region Movement

   public void UpdateArrowIndicators()
{
  

    // 연결 노드 순서대로 활성화 및 위치/방향 세팅
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

    void MoveToDirection(Vector3 dragDirection) // 카메라 기준으로 동서남북 설정
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

        if (targetNode != null)
        {
           

            StartCoroutine(MoveToPosition(targetNode.transform.position));

            currentNode = targetNode;
            if (currentNode.isGoalNode)
            {
                #region TEMP
                // Level finish Event
                Debug.Log("도착~~~~");
                #endregion
            }
        }

        else
        {
            Debug.Log("Can not move because no target Node.");
        }
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

}
