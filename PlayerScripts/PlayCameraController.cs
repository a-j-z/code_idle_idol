using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraController : MonoBehaviour
{
    public GameObject player;
    public float lookAheadX;
    public float lookAheadY;
    public float smooth;

    private BoundsInt bounds;
    private Camera cam;
    private float camSize;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();
        camSize = cam.orthographicSize;
        if (cam.aspect > 2.0f/1.0f) 
        {
            camSize *= (2.0f/1.0f) / cam.aspect; 
            cam.orthographicSize = camSize;
        }
        GoToDestination(player.GetComponent<Rigidbody2D>().position);
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position, CalculateDestination(player.GetComponent<Rigidbody2D>().position), ref velocity, smooth);
    }

    private Vector3 CalculateDestination(Vector3 target)
    {
        Vector3 destination;
        destination = target + Vector3.back * 10f +
            Vector3.right * lookAheadX * player.GetComponent<Rigidbody2D>().velocity.x +
            Vector3.up * lookAheadY * player.GetComponent<Rigidbody2D>().velocity.y;

        if (destination.y > bounds.yMax - camSize - 1.0f) 
            destination = new Vector3(destination.x, bounds.yMax - camSize - 1.0f, destination.z);
        if (destination.y < bounds.yMin + camSize) 
            destination = new Vector3(destination.x, bounds.yMin + camSize, destination.z);
        if (destination.x > bounds.xMax - cam.aspect * camSize - 1.0f) 
            destination = new Vector3(bounds.xMax - cam.aspect * camSize - 1.0f, destination.y, destination.z);
        if (destination.x < bounds.xMin + cam.aspect * camSize) 
            destination = new Vector3(bounds.xMin + cam.aspect * camSize, destination.y, destination.z);
        return destination;
    }

    public void UpdateBounds(BoundsInt bounds)
    {
        this.bounds = bounds;
    }

    public void GoToDestination(Vector3 target)
    {
        transform.position = CalculateDestination(target);
    }
}
