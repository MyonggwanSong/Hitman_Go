using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node Connecting!")]
    public List<Node> connectedNodes = new List<Node>();
    [Header("Goal Node")]
    [SerializeField] public bool isGoalNode = false;
    [Header("Node Materials")]
    [SerializeField] Material normalNodeMaterial;
    [SerializeField] Material goalNodeMaterial;

    [HideInInspector] public bool isLineDrawn = false; // 이미 라인 그렸는지 여부
    MeshRenderer meshrenderer;

    void OnValidate()
    {
        meshrenderer = GetComponentInChildren<MeshRenderer>();
        meshrenderer.material = isGoalNode ? goalNodeMaterial : normalNodeMaterial;
    }


}
