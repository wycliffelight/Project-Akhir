using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackroundScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;

    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    void Start()
    {
        offset.z = transform.position.z;
    }

    void LateUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
    }

}
