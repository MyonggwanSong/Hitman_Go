using UnityEngine;
using CustomInspector;

public class ItemSpawner : MonoBehaviour
{
    [Title("Spawn Node")]
    [SerializeField] Node SpawnNode;    // 스폰되는 노드

    [Title("Target Nodes")]
    [SerializeField] Node[] TargetNodes;   // 던질수 있는 노드

    [Title("Item Prefab")]
    [SerializeField] Item Item;
    [Title("Indicator Prefab")]
    [SerializeField] GameObject Indicator_Targetable;
    [SerializeField] GameObject Indicator_Range;

 void Start()
    {
        Item spawnedItem = Instantiate(Item, SpawnNode.transform.position, transform.rotation);
        SetInitialItem(spawnedItem);

    }
    void SetInitialItem(Item clone)
    {
        clone.indicatorPrefab = Indicator_Targetable;
        clone.indicatorRangePrefab = Indicator_Range;
        clone.targetNodes = TargetNodes;
        clone.currentNode = SpawnNode;
        clone.currentNode.hasItem = true;
    }
}
