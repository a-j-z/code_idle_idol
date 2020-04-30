using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectDraw : MonoBehaviour
{
    private GameObject[] fillArea = new GameObject[4];
    private LineRenderer[] lines = new LineRenderer[4];

    void Start()
    {
        SetupRectDraw();
    }

    private void SetupRectDraw()
    {
        for (int i = 0; i < 4; i++)
        {
            fillArea[i] = new GameObject("Line" + (i + 1));
            fillArea[i].transform.position = new Vector3(0, 0, 1);
            lines[i] = fillArea[i].AddComponent<LineRenderer>();
            lines[i].startWidth = 0.2f; lines[i].endWidth = 0.2f;
            lines[i].material = new Material(Shader.Find("UI/Default"));
            lines[i].material.color = Color.black;
            lines[i].useWorldSpace = true;
            lines[i].positionCount = 2;
        }
        for (int i = 0; i < 4; i++)
        {
            fillArea[i].transform.parent = transform;
        }
    }

    public void Draw(Vector3Int start, Vector3Int end)
    {
        float isStartXMax = start.x > end.x ? 1 : -1;
        float isStartYMax = start.y > end.y ? 1 : -1;
        float bit = lines[0].startWidth / 2f;

        lines[0].SetPosition(0, new Vector3(start.x + isStartXMax * (0.5f + bit), start.y + isStartYMax * 0.5f, 0));
        lines[0].SetPosition(1, new Vector3(end.x - isStartXMax * (0.5f + bit), start.y + isStartYMax * 0.5f, 0));

        lines[1].SetPosition(0, new Vector3(start.x + isStartXMax * (0.5f + bit), end.y - isStartYMax * 0.5f, 0));
        lines[1].SetPosition(1, new Vector3(end.x - isStartXMax * (0.5f + bit), end.y - isStartYMax * 0.5f, 0));

        lines[2].SetPosition(0, new Vector3(start.x + isStartXMax * 0.5f, start.y + isStartYMax * (0.5f + bit), 0));
        lines[2].SetPosition(1, new Vector3(start.x + isStartXMax * 0.5f, end.y - isStartYMax * (0.5f + bit), 0));

        lines[3].SetPosition(0, new Vector3(end.x - isStartXMax * 0.5f, start.y + isStartYMax * (0.5f + bit), 0));
        lines[3].SetPosition(1, new Vector3(end.x - isStartXMax * 0.5f, end.y - isStartYMax * (0.5f + bit), 0));
    }
}
