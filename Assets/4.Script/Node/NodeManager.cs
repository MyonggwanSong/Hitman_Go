using System.Collections;
using UnityEngine;
using CustomInspector;

public class NodeManager : MonoBehaviour
{
    [Title("Line Renderer Attribute")]
    public float drawLineSpeed = 2f;
    public float drawLineThickness = 0.05f;
    
    [Title("Start Node")]
    [SerializeField] Node startingNode;
    private DrawLine dl;
 void Awake()
    {
        dl = startingNode.GetComponent<DrawLine>();
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(GameManager.I.startDelay);
        GameManager.I.isGameStart = true; // 게임 시작

        startingNode.mesh.gameObject.SetActive(true);
         if (startingNode != null)
        {
            dl.InitializeLineRenderers();
            dl.DrawConnectionsSimultaneously();
            startingNode.isLineDrawn = true;
        }
    }

}
