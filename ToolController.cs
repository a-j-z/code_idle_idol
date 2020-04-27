using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolController : MonoBehaviour
{
    public LevelDraw currentTool;
    public int toolId;

    public Sprite selected;
    private Sprite unselected;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        unselected = image.sprite;
    }

    void Update()
    {
        if (currentTool.GetTool() == toolId)
        {
            image.sprite = selected;
        }
        else
        {
            image.sprite = unselected;
        }
    }
}
