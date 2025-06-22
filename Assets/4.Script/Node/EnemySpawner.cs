using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Node SpawnNode;    // 스폰되는 노드
    [SerializeField] Node TargetNode;   // AI 목적지 노드
    [SerializeField] Transform deathZone;

    [SerializeField] EnemyControl Enemy;


   
    void Start()
    {
        Vector3 direction = (Camera.main.transform.forward - Camera.main.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;
        Quaternion rotation = Quaternion.Euler(direction);

        Enemy.currentNode = SpawnNode;
        Enemy.targetNode = TargetNode;
        Enemy.deathZone = deathZone;
        Instantiate(Enemy, SpawnNode.transform.position, rotation);
        
       
    }
}
