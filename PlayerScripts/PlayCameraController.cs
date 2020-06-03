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
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.position = player.transform.position + Vector3.back * 10f;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 destination;
        destination = (Vector3)player.GetComponent<Rigidbody2D>().position + Vector3.back * 10f +
                Vector3.right * lookAheadX * player.GetComponent<Rigidbody2D>().velocity.x +
            Vector3.up * lookAheadY * player.GetComponent<Rigidbody2D>().velocity.y;

        if (destination.y > bounds.yMax - 8.0f - 1.0f) destination = new Vector3(destination.x, bounds.yMax - 8.0f - 1.0f, destination.z);
        if (destination.y < bounds.yMin + 8.0f) destination = new Vector3(destination.x, bounds.yMin + 8.0f, destination.z);
        if (destination.x > bounds.xMax - cam.aspect * 8.0f - 1.0f) destination = new Vector3(bounds.xMax - cam.aspect * 8.0f - 1.0f, destination.y, destination.z);
        if (destination.x < bounds.xMin + cam.aspect * 8.0f) destination = new Vector3(bounds.xMin + cam.aspect * 8.0f, destination.y, destination.z);
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smooth);
    }

    public void UpdateBounds(BoundsInt bounds)
    {
        this.bounds = bounds;
    }
}
