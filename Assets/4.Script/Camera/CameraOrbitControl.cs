using UnityEngine;

public class CameraOrbitControl : MonoBehaviour
{
    public Transform target;       // 카메라가 바라볼 중심 (보드 중심 Transform)
    public float distance = 10.0f; // 보드로부터의 거리
    public float orbitSpeed = 5.0f;
    public float smoothTime = 0.2f;

    private Vector3 currentRotation;
    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 originalRotation; // 원래 카메라 각도 기억용
    private bool isOrbiting = false;

    void Start()
    {
        currentRotation = transform.eulerAngles;
        originalRotation = currentRotation; // 시작 시 카메라 회전 저장
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverPlayer())
            {
                isOrbiting = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isOrbiting = false;
        }

        if (isOrbiting)
        {
            float mouseX = Input.GetAxis("Mouse X") * orbitSpeed;
            float mouseY = -Input.GetAxis("Mouse Y") * orbitSpeed;

            currentRotation.x += mouseY;
            currentRotation.y += mouseX;

            // 상하 회전 제한
            currentRotation.x = Mathf.Clamp(currentRotation.x, 10f, 80f);
        }
        else
        {
            // Orbit 종료 시 원래 각도로 복귀
            currentRotation = Vector3.SmoothDamp(currentRotation, originalRotation, ref smoothVelocity, smoothTime);
        }

        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        transform.position = target.position - rotation * Vector3.forward * distance;
        transform.LookAt(target);
    }

    bool IsPointerOverPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
}
