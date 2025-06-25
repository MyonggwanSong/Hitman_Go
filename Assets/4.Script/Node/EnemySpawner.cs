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
    [SerializeField] int mesheIndex;
    
    void Start()
    {
        EnemyControl spawnedEnemy = Instantiate(Enemy, SpawnNode.transform.position, transform.rotation);
        SetInitialEnemy(spawnedEnemy);

      
    }

    void SetInitialEnemy(EnemyControl clone)
    {
        clone.currentNode = SpawnNode;
        if (TargetNode != null)
        {
            clone.targetNode = TargetNode;

            Vector3 lookDir = new Vector3(clone.targetNode.transform.position.x, transform.position.y, clone.targetNode.transform.position.z);
            clone.transform.LookAt(lookDir);
        }
        

        clone.deathZone = deathZone;
        clone.mesheIndex = mesheIndex;

        clone.SetInitial();
    }
}
