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
    Vector3 lookDir = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
    Quaternion rotation = Quaternion.Euler(lookDir);

    PlayerControl spawnedPlayer = Instantiate(player, startingNode.transform.position, rotation);
    spawnedPlayer.transform.LookAt(lookDir);

    spawnedPlayer.currentNode = startingNode;
    spawnedPlayer.allEnemies = FindObjectsOfType<EnemyControl>();   // 처치할 적 
    GameManager.I.enemyNum = spawnedPlayer.allEnemies.Length;

    // 직접 초기화 호출
    spawnedPlayer.InitializeArrowIndicators();
}
}
