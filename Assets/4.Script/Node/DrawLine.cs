using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private Node node;
    private NodeManager nodeManager;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public Material lineMaterial;

    void Awake()
    {
        nodeManager = GetComponentInParent<NodeManager>();
        if (nodeManager == null)
            Debug.LogError("NodeManager is missing :");
        node = GetComponent<Node>();
        if (node == null)
            Debug.LogError("Node Component is missing ");


    }

    public void InitializeLineRenderers()
    {
        lineRenderers.Clear();

        foreach (Node targetNode in node.connectedNodes)
        {
            if (targetNode == null || targetNode == node) continue;
            if (targetNode.isLineDrawn) continue; //  이미 그린 노드면 스킵

            GameObject lrObj = new GameObject("LineTo_" + targetNode.name);
            lrObj.transform.parent = this.transform;

            LineRenderer lr = lrObj.AddComponent<LineRenderer>();
            lrObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            lr.alignment = LineAlignment.TransformZ;    // 카메라 따라다니지 않게 눕히기
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));   // 머테리얼 없으면 디폴트 머테리얼 찾아서 넣기
            lr.startWidth = nodeManager.drawLineThickness; 
            lr.endWidth = nodeManager.drawLineThickness;
            lr.positionCount = 0;

            lineRenderers.Add(lr);
        }
    }

    public void DrawConnectionsSimultaneously()
    {
        int rendererIndex = 0;

        for (int i = 0; i < node.connectedNodes.Count; i++)
        {
            Node targetNode = node.connectedNodes[i];
            if (targetNode == null || targetNode == node) continue; // 없거나 본인이면 스킵
            if (targetNode.isLineDrawn) continue; // 이미 그린 노드면 그리지 않음

            StartCoroutine(DrawLineToNode(targetNode, lineRenderers[rendererIndex]));
            rendererIndex++; // 사용한 LineRenderer만 인덱스 증가
        }
    }

    private IEnumerator DrawLineToNode(Node targetNode, LineRenderer lr)
    {
        Vector3 startPoint = transform.position;
        Vector3 endPoint = targetNode.transform.position;
        Vector3 direction = (endPoint - startPoint).normalized;
        
        float normalNodeRadius = 0.1f; 
        float goalNodeRadius = 0.6f;

        Node nodeFrom = GetComponent<Node>();
        if (nodeFrom.isGoalNode)
            startPoint += direction * goalNodeRadius;


        float nodeRadius = targetNode.isGoalNode ? goalNodeRadius : normalNodeRadius; // 노드의 크기많큼 덜 그리기위함
        startPoint += direction * normalNodeRadius;
        endPoint -= direction * nodeRadius;

        float t = 0f;
        

        lr.positionCount = 2;
        lr.SetPosition(0, startPoint);
        while (t < 1f)
        {
            t += Time.deltaTime * nodeManager.drawLineSpeed;
            Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);
            lr.SetPosition(1, currentPos);
            
            yield return null;
        }

        lr.SetPosition(1, endPoint);
        // 노드 켜주기
        targetNode.mesh.gameObject.SetActive(true);
        // 그렸다고 표시
        targetNode.isLineDrawn = true;

        // 다음 노드 라인 그리기
        DrawLine nextDrawLine = targetNode.GetComponent<DrawLine>();
        if (nextDrawLine != null)
        {
            nextDrawLine.InitializeLineRenderers();
            nextDrawLine.DrawConnectionsSimultaneously();
        }
    }
}