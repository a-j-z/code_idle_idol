using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Camera cam;
    public GameObject target;
    public Vector2 offset;

    private RectTransform rt;
    private Vector2 screenPoint;
    private Canvas canvas;
    private RectTransform canvasRT;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        screenPoint = RectTransformUtility.WorldToScreenPoint(cam, target.transform.position);
        canvas = GetComponentInParent<Canvas>();
        canvasRT = canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        rt = GetComponent<RectTransform>();
        screenPoint = RectTransformUtility.WorldToScreenPoint(cam, target.transform.position);
        canvas = GetComponentInParent<Canvas>();
        canvasRT = canvas.GetComponent<RectTransform>();
        rt.anchoredPosition = screenPoint - canvasRT.sizeDelta / 2f + offset;
    }
}
