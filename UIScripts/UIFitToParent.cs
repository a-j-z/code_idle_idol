using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFitToParent : MonoBehaviour
{
    public RectTransform reference;
    public float widthOffset;
    public float heightOffset;

    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.sizeDelta = reference.sizeDelta;
        rt.sizeDelta += new Vector2(widthOffset, heightOffset);
    }
}
