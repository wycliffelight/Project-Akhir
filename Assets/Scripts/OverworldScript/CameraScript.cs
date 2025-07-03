using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform target;  
    public Vector3 offset;   
    public float smoothTime = 0.1f;  

    private Vector3 velocity = Vector3.zero;  // Untuk camera smooth
    private Camera cam; 

    public float zoomSpeed = 2f; 
    public float minZoom = 5f;  
    public float maxZoom = 20f;  

    public float minX = -10f;     
    public float maxX = 10f;      

    void Start()
    {
        // Refrensikan kamera
        cam = Camera.main;

        // Memastikan initial Z position camera tetap yang di atur dengan offset
        offset.z = transform.position.z;
    }

    void LateUpdate()
    {
        FollowTarget();
    }


    void Update()
    {
        HandleZoom();
    }

    void FollowTarget()
    {
        // Kalkulasi posisi target kamera
        Vector3 targetPosition = target.position + offset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        // Mengerakan kamera menurut SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Memastikan lagi initial Z position camera tetap yang di atur dengan offset
        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
    }

    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.Equals)) 
        {
            cam.orthographicSize -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            cam.orthographicSize += zoomSpeed * Time.deltaTime;
        }

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
