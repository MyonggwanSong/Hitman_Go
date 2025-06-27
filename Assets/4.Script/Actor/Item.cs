using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;
public class Item : MonoBehaviour
{
    [Title("Event Connect!!")]
    [SerializeField] EventDetect eventDetect;
    [ReadOnly] public GameObject indicatorPrefab;
    [ReadOnly] public GameObject indicatorRangePrefab;

    [ReadOnly] public Node[] targetNodes;
    [ReadOnly] public Node targetNode;
    [ReadOnly] public Node currentNode;
    [ReadOnly] public MeshRenderer[] meshs;


    private List<GameObject> indicators = new List<GameObject>();


    public float flightTime = 1f; // 날아가는 전체 시간
    public float height = 2f;     // 포물선 최고 높이
    public bool isTargetItem = false;

    private Vector3 startPos;

    void Awake()
    {
        meshs = GetComponentsInChildren<MeshRenderer>();

    }
    void Start()
    {
        if (!isTargetItem)
            meshs[0].gameObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        ChangeMesh(true);
        if (other.tag == "Player")
        {
            if (isTargetItem)
            {
                GameManager.I.getTargetItem = true;
                meshs[0].gameObject.SetActive(false);
            }
            else
            {
                other.GetComponentInParent<PlayerControl>().hasItem = this;
                IsPicked();
            }
        }
    }

    void ChangeMesh(bool b)
    {
        if (!isTargetItem)
        {
            meshs[0].gameObject.SetActive(b);
            meshs[1].gameObject.SetActive(!b);
        }

    }

    public void IsPicked()
    {

        foreach (Node n in targetNodes)
        {
            indicators.Add(Instantiate(indicatorPrefab, n.transform.position + Vector3.up * 0.02f, Quaternion.Euler(new Vector3(90, 0, 0))));
        }
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
        StartCoroutine(AnimationRangeIndicator());

        eventDetect.occuredNode = targetNode;
        eventDetect.Raise();


        Debug.Log("돌이 목표 지점에 도착!");

        // 필요하면 이곳에서 폭발, 적 유도, 사운드 재생 가능
    }

    public IEnumerator AnimationRangeIndicator()
    {
        GameObject clone = Instantiate(indicatorRangePrefab, transform.position, Quaternion.identity, transform);
        //clone.GetComponent<Animation>().Play("RangeIndicatorAnimation");

        yield return new WaitForSeconds(1f);
        Destroy(clone);
        gameObject.SetActive(false);
        yield return null;
    }
}
