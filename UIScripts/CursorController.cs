using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Camera cam;

    private Vector3 worldPoint;
    private Vector2 screenPoint;
    private float cursorSize;

    private RectTransform rt;
    private Canvas canvas;
    private RectTransform canvasRt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRt = canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        worldPoint = MouseUtilities.GridSpace(cam);
        screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);

        cursorSize = Screen.height / (cam.orthographicSize * 2);
        rt.sizeDelta = new Vector2(cursorSize, cursorSize);
        rt.anchoredPosition = screenPoint - canvasRt.sizeDelta / 2f;
    }
}
