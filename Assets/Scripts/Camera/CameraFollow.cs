using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; 

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.05f; 
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Map Bounds")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPosition = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxPosition = new Vector2(10f, 10f);

    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}