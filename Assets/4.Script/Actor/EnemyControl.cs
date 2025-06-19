using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public Camera mainCamera;
    public float moveDistance = 5f;
    public float moveSpeed = 5f;

    public Node currentNode; 
    public Node nextNode;
    public List<Node> aiRootNodes = new List<Node>();

    private bool isMoving = false;

    void Start()
    {
        int rnd = Random.Range(0, currentNode.connectedNodes.Count);
        nextNode = currentNode.connectedNodes[rnd];
    }
    public void OnTurnChanged(uint newTurn)
    {
        // 턴 넘어갔을 때 AI 동작
        AiMove();
            
    }


    void AiMove()
    {
        if (currentNode.connectedNodes == null || currentNode.connectedNodes.Count == 0)
        {
            Debug.LogWarning("No connected nodes to move.");
            return;
        }
        OnEnemyTurn();

        int rnd = Random.Range(0, currentNode.connectedNodes.Count);
        nextNode = currentNode.connectedNodes[rnd];
    }

    void OnEnemyTurn() // AI가 직접 이동
    {
        if (nextNode != null)
        {
            StartCoroutine(MoveToPosition(nextNode.transform.position));
            currentNode = nextNode;
        }
        else
        {
            Debug.Log("Cannot move: No target Node.");
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
