using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovingScript : MonoBehaviour
{
    public float moveX = 0f;  
    public float moveY = 0f;
    public float speed = 2f;  
    public float gizmoOffsetX = 0f; 
    public float gizmoOffsetY = 0f; 

    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool movingToEnd = true;
    private Vector3 lastPosition;
        private Rigidbody2D playerRigidbody;

    void Start()
    {
        // Menyimpan posisi awal dan kalkulasi posisi akhir
        startPosition = transform.position;
        endPosition = new Vector3(startPosition.x + moveX, startPosition.y + moveY, startPosition.z);
        lastPosition = transform.position;
    }


    void Update()
    {
        Vector3 newPosition;

        if (movingToEnd)
        {
            newPosition = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
            if (newPosition == endPosition)
            {
                movingToEnd = false;
            }
        }
        else
        {
            newPosition = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            if (newPosition == startPosition)
            {
                movingToEnd = true;
            }
        }

        Vector3 deltaPosition = newPosition - lastPosition;
        if (playerRigidbody != null)
        {
            playerRigidbody.position += (Vector2)deltaPosition;
        }

        transform.position = newPosition;
        lastPosition = transform.position;
    }


    // Membuat player mengikuti platform dengan cara menambahkan rigidbody ke player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerRigidbody = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerRigidbody = null;
        }
    }

    // Utility
    private void OnDrawGizmos()
    {
        Vector3 gizmoStartPosition = new Vector3(startPosition.x + gizmoOffsetX, startPosition.y + gizmoOffsetY, startPosition.z);
        Vector3 gizmoEndPosition = new Vector3(endPosition.x + gizmoOffsetX, endPosition.y + gizmoOffsetY, endPosition.z);

        if (!Application.isPlaying)
        {
            startPosition = transform.position;
            endPosition = new Vector3(startPosition.x + moveX, startPosition.y + moveY, startPosition.z);
            gizmoStartPosition = new Vector3(startPosition.x + gizmoOffsetX, startPosition.y + gizmoOffsetY, startPosition.z);
            gizmoEndPosition = new Vector3(endPosition.x + gizmoOffsetX, endPosition.y + gizmoOffsetY, endPosition.z);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(gizmoStartPosition, gizmoEndPosition);
        Gizmos.DrawSphere(gizmoStartPosition, 0.1f);
        Gizmos.DrawSphere(gizmoEndPosition, 0.1f);
    }
}
