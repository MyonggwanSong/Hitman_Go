using UnityEngine;


public class NodeCameraMoving : MonoBehaviour
{
    [SerializeField] Vector3 cameraMove2;
    [SerializeField] Vector3 cameraRotate2;

    [SerializeField] float duration;

    private Vector3 previousCamPos;
    private Quaternion previousCamRot;
    private Coroutine _co;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("들어옴");
            previousCamPos = Camera.main.transform.position;
            previousCamRot = Camera.main.transform.rotation;
            _co = StartCoroutine(Camera.main.GetComponent<CameraMove>().MoveCamera(cameraMove2, Quaternion.Euler(cameraRotate2), duration));
        }
    }
    void OnTriggerExit(Collider other)
    {
        StopCoroutine(_co);
        if (other.tag == "Player")
            StartCoroutine(Camera.main.GetComponent<CameraMove>().MoveCamera(previousCamPos, previousCamRot, duration));

    }



}
