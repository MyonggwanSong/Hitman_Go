using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Node startingNode;
    [SerializeField] PlayerControl player;


    void Start()
    {
        Vector3 direction = (Camera.main.transform.forward - Camera.main.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;
        Quaternion rotation = Quaternion.Euler(direction);
        Instantiate(player, startingNode.transform.position, rotation);
        player.currentNode = startingNode;
        if (startingNode != null)
        {
            DrawLine dl = startingNode.GetComponent<DrawLine>();
            dl.InitializeLineRenderers();
            dl.DrawConnectionsSimultaneously();
            startingNode.isLineDrawn = true;
        }
    }
}
