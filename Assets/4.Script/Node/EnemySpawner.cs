using UnityEngine;
using CustomInspector;

public class EnemySpawner : MonoBehaviour
{
    [Title("Spawn Node")]
    [SerializeField] Node SpawnNode;    // 스폰되는 노드

    [Title("(AI Route) Target Node")]
    [SerializeField] Node TargetNode;   // AI 목적지 노드
    
    [Title("Tomb")]
    [SerializeField] Transform deathZone;
    
    [Title("Enmey Prefab")]
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
