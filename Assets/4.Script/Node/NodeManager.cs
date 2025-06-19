using System.Collections;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public float drawLineSpeed = 2f;
    public float drawLineThickness = 0.05f;
    [SerializeField] Node startingNode;
    DrawLine dl;
 void Awake()
    {
        dl = startingNode.GetComponent<DrawLine>();
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(GameManager.I.startDelay);
        startingNode.gameObject.SetActive(true);
         if (startingNode != null)
        {

            dl.InitializeLineRenderers();
            dl.DrawConnectionsSimultaneously();
            startingNode.isLineDrawn = true;
        }
    }

}
