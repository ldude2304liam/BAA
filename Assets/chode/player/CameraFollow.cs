using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    // pulled back on Z for 2D
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}