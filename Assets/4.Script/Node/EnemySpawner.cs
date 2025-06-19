using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Node SpawnNode;
    [SerializeField] EnemyControl Enemy;


   
    void Start()
    {
        Vector3 direction = (Camera.main.transform.forward - Camera.main.transform.position).normalized;
        direction.x = 0f;
        direction.z = 0f;
        Quaternion rotation = Quaternion.Euler(direction);

        Instantiate(Enemy, SpawnNode.transform.position, rotation);
        Enemy.currentNode = SpawnNode;
        
       
    }
}
