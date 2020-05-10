using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSensitivity;
    public float maxSize;
    public float minSize;
    public PaletteMenuManager paletteMenuManager;

    private Camera cam;
    private Vector3 panStart;


    void Start()
    {
        cam = GetComponent<Camera>();
        panStart = Vector3.zero;
    }

    void Update()
    {
        if (!paletteMenuManager.IsPaletteMenuActive())
        {
            cam.orthographicSize *= 1.0f - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            if (cam.orthographicSize > maxSize) cam.orthographicSize = maxSize;
            if (cam.orthographicSize < minSize) cam.orthographicSize = minSize;

            if (Input.GetMouseButton(2))
            {
                transform.position += panStart - MouseUtilities.WorldSpace(cam);
            }
            panStart = MouseUtilities.WorldSpace(cam);
        }
    }
}
