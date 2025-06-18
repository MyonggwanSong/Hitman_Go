using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Camera mainCamera;
    public float moveDistance = 5f;
    public float moveSpeed = 5f;

    private Vector3 dragStartPos;
    private Vector3 dragEndPos;
    private bool isDragging = false;

    public Node currentNode; // 현재 위치한 노드

    void Awake()
    {
        mainCamera = Camera.main;
    }
   

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPos = Input.mousePosition;
            Vector3 dragVector = dragEndPos - dragStartPos;

            if (dragVector.magnitude > 10f)
            {
                MoveToDirection(dragVector.normalized);
            }

            isDragging = false;
        }
    }

    void MoveToDirection(Vector3 dragDirection)
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

        // 현재 노드의 연결된 노드들 중, 이동 방향과 가장 가까운 노드를 찾음
        Node targetNode = GetConnectedNodeInDirection(targetDirection);

        if (targetNode != null)
        {
            StartCoroutine(MoveToPosition(targetNode.transform.position));
            currentNode = targetNode; // 노드 갱신
        }
        else
        {
            Debug.Log("이 방향으로 연결된 노드 없음!");
        }
    }

    Node GetConnectedNodeInDirection(Vector3 direction)
    {
        Node closestNode = null;
        float maxDot = 0.7f; // 허용 각도 (cos(45도) = 약 0.7)

        foreach (Node node in currentNode.connectedNodes)
        {
            Vector3 dirToNode = (node.transform.position - currentNode.transform.position).normalized;
            float dot = Vector3.Dot(direction, dirToNode);

            if (dot > maxDot) // threshold 넘는 방향만 인정
            {
                maxDot = dot;
                closestNode = node;
            }
        }

        return closestNode;
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        transform.position = targetPos;
    }
}
