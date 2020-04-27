using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSensitivity;

    private Camera cam;
    private Vector3 panStart;


    void Start()
    {
        cam = GetComponent<Camera>();
        panStart = Vector3.zero;
    }

    void Update()
    {
        cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        
        if (Input.GetMouseButton(2))
        {
            transform.position += panStart - MouseUtilities.WorldSpace(cam);
        }
        panStart = MouseUtilities.WorldSpace(cam);
    }
}
