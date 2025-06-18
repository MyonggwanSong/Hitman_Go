using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Node startingNode;
    [SerializeField] PlayerControl player;


    void Start()
    {
        
        if (startingNode != null)
        {
            DrawLine dl = startingNode.GetComponent<DrawLine>();
            dl.InitializeLineRenderers();
            dl.DrawConnectionsSimultaneously();
            startingNode.isLineDrawn = true;
        }
    }
}
