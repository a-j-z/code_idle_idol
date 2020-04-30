using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolController : MonoBehaviour
{
    public LevelDraw draw;
    [TextArea(5, 15)]
    public string[] toolInfo;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = toolInfo[draw.GetTool() - 1];
    }
}
