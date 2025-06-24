using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;
public class Item : MonoBehaviour
{
     [Title("Event Connect!!")]
    [SerializeField] EventDetect eventDetect;
    [ReadOnly] public GameObject indicatorPrefab;
    [ReadOnly] public Node[] targetNodes;
    [ReadOnly] public Node targetNode;
    [ReadOnly] public Node currentNode;


    private List<GameObject> indicators = new List<GameObject>();

    
    public float flightTime = 1f; // 날아가는 전체 시간
    public float height = 2f;     // 포물선 최고 높이
    public bool isTargetItem = false;

    private Vector3 startPos;


    

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isTargetItem)
            {
                Destroy(this);
                GameManager.I.getTargetItem = true;
            }
            else
            {
                other.GetComponentInParent<PlayerControl>().hasItem = this;
                IsPicked();
                Debug.Log("들어옴");
            }
        }
    }

    public void IsPicked()
    {

        foreach (Node n in targetNodes)
        {
            indicators.Add(Instantiate(indicatorPrefab, n.transform.position + Vector3.up * 0.02f, Quaternion.Euler(new Vector3(90, 0, 0))));
        }
        
        //return true;
    }


    public IEnumerator ParabolaMoveCoroutine()
    {

        startPos = transform.position;

        float elapsed = 0f;

        while (elapsed < flightTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flightTime);

            // 가로 방향 보간
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetNode.transform.position, t);

            // 세로 방향 포물선 곡선
            float parabolicY = Mathf.Sin(Mathf.PI * t) * height;

            // 최종 위치
            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + parabolicY, horizontalPos.z);

            yield return null;
        }


        // 마지막 위치 보정 (혹시나 덜 도달했을 경우)
        transform.position = targetNode.transform.position;

        // 여기서 충돌 판정 or 효과 처리 가능
        OnArrived();
    }

    private void OnArrived()
    {
        foreach (var i in indicators)
        {
            Destroy(i);
        }

        eventDetect.occuredNode = targetNode;
        eventDetect.Raise();


        Debug.Log("돌이 목표 지점에 도착!");
        gameObject.SetActive(false);
        // 필요하면 이곳에서 폭발, 적 유도, 사운드 재생 가능
    }
}
