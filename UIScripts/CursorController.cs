using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Camera cam;
    public LevelDraw draw;

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

        cursorSize = (Screen.height / 100f) / (cam.orthographicSize * 2);
        rt.localScale = new Vector3(cursorSize, cursorSize, 0);
        rt.sizeDelta = draw.GetCurrentPaletteSize();
        rt.anchoredPosition = screenPoint - canvasRt.sizeDelta / 2f + new Vector2(
            (rt.sizeDelta.x % 200 == 0) ? (50f * rt.localScale.x) : 0, (rt.sizeDelta.y % 200 == 0) ? (50f * rt.localScale.y) : 0);
    }
}
