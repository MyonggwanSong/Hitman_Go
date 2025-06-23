using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public class Node : MonoBehaviour
{
    [HorizontalLine("Connect Nodes!!",color:FixedColor.Gray), HideField] public bool _l0;
    public List<Node> connectedNodes = new List<Node>();
    [HorizontalLine(color:FixedColor.Gray), HideField] public bool _l1;

    [Title("Goal Node")]
    [SerializeField] public bool isGoalNode = false;

    [Title("Node Materials")]
    [SerializeField] Material normalNodeMaterial;
    [SerializeField] Material goalNodeMaterial;

    [ReadOnly] public bool hasItem;         // 아이템 있음?
    [ReadOnly] public bool hasBush;         // 숨을 수 있음?

    [HideInInspector] public bool isLineDrawn = false; // 이미 라인 그렸는지 여부
    [HideInInspector] public MeshRenderer mesh;
    MeshRenderer meshrenderer;
    void Awake()
    {
        connectedNodes.RemoveAll(node => node == null); // 지워진 노드 또는 연결이 안 되는 노드는 List에서 삭제
        mesh = GetComponentInChildren<MeshRenderer>();      // animation casing
        if (mesh == null)
            Debug.LogWarning("Node ] mesh 없음");
        mesh.gameObject.SetActive(false);
    }



    void OnValidate()
    {
        meshrenderer = GetComponentInChildren<MeshRenderer>();
        meshrenderer.material = isGoalNode ? goalNodeMaterial : normalNodeMaterial;
    }



}
