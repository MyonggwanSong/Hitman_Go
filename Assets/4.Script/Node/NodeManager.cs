using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public float drawLineSpeed = 2f;
    public float drawLineThickness = 0.05f;
    [SerializeField] PlayerSpawner startingNode;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(GameManager.I.startDelay);
        startingNode.gameObject.SetActive(true);
    }

}
