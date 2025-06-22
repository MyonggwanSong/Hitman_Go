using UnityEngine;
using CustomInspector;

public class PlayerSpawner : MonoBehaviour
{
    [Title("Starting Node")]
    [SerializeField] Node startingNode;
     [Title("Player Prefab")]

    [SerializeField] PlayerControl player;


   
    void Start()
    {
        Vector3 direction = (Camera.main.transform.forward - Camera.main.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;
        Quaternion rotation = Quaternion.Euler(direction);

        Instantiate(player, startingNode.transform.position, rotation);
        player.currentNode = startingNode;
        
       player.allEnemies = FindObjectsOfType<EnemyControl>();
       GameManager.I.enemyNum = player.allEnemies.Length;
    }
}
