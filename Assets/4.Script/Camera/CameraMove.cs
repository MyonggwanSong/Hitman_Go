using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }

}
