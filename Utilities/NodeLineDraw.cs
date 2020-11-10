using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeLineDraw : MonoBehaviour
{
    public Sprite node;
    public Camera cam;
    public LevelDraw draw;
    
    private GameObject line;
    private LineRenderer lineRenderer;
    private GameObject[] nodes = new GameObject[2];
    private bool isSetup = false;

    private Vector3 mousePositionBuffer = Vector3.zero;
    private int currentlyDragged;
    private bool isSelected;

    void Start()
    {
        if (!isSetup) SetupNodeLineDraw();
    }

    void Update()
    {
        if (PlayManager.GetIsPlay() || !isSelected)
        {
            lineRenderer.enabled = false;
            for (int i = 0; i < nodes.Length; i++) nodes[i].SetActive(false);
        }
        else
        {
            lineRenderer.enabled = true;
            for (int i = 0; i < nodes.Length; i++) nodes[i].SetActive(true);
        }
        isSelected = gameObject.name.Equals(draw.GetLayer());
        if (Input.GetMouseButtonDown(0) && isSelected) mousePositionBuffer = MouseUtilities.WorldSpace(cam);
        if (Input.GetMouseButton(0) && isSelected)
        {
            Vector3 positionChange = MouseUtilities.WorldSpace(cam) - mousePositionBuffer;
            for (int i = 0; i < nodes.Length; i++)
            {
                Vector3 normalizedPosition = new Vector3(nodes[i].transform.position.x, nodes[i].transform.position.y, 0);
                if ((Vector3.Distance(MouseUtilities.WorldSpace(cam), normalizedPosition) < 0.5f && currentlyDragged == -1) ||
                    currentlyDragged == i)
                {
                    nodes[i].transform.position += positionChange;
                    lineRenderer.SetPosition(i, nodes[i].transform.position + Vector3.forward);
                    currentlyDragged = i;
                    break;
                }
            }   
            mousePositionBuffer = MouseUtilities.WorldSpace(cam);
        }
        if (Input.GetMouseButtonUp(0) && isSelected)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Vector3 pos = nodes[i].transform.position;
                nodes[i].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);
                lineRenderer.SetPosition(i, nodes[i].transform.position + Vector3.forward);
            }
            currentlyDragged = -1;
        }
    }

    private void SetupNodeLineDraw()
    {
        line = new GameObject("Line");
        lineRenderer = new LineRenderer();
        currentlyDragged = -1;
        isSelected = false;

        isSetup = true;
        line.transform.position = new Vector3(0, 0, 1);
        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f; lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("UI/Default"));
        lineRenderer.material.color = Color.black;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        line.transform.parent = transform;

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GameObject("Node" + (i+1));
            SpriteRenderer spriteRenderer = nodes[i].AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = node;
            if (i == 0) spriteRenderer.color = new Color(0f,0f,0f,1f);
            nodes[i].transform.parent = transform;
        }

        lineRenderer.SetPosition(0, new Vector3(-1, 0, 0));
        lineRenderer.SetPosition(1, new Vector3(1, 0, 0));
        nodes[0].transform.position = new Vector3(-1, 0, 0);
        nodes[1].transform.position = new Vector3(1, 0, 0);
    }

    public Vector3 GetNodeLocation(int nodeIndex)
    {
        return nodes[nodeIndex].transform.position;
    }

    public void SetSpeed(float speed)
    {
        if (!isSetup) SetupNodeLineDraw();
        transform.GetComponentInChildren<MoveAlongLine>().speed = speed;
    }

    public void SetNodeAX(float xPos)
    { 
        if (!isSetup) SetupNodeLineDraw();
        Vector3 pos = nodes[0].transform.position; 
        nodes[0].transform.position = new Vector3(xPos, pos.y, pos.z);
    }

    public void SetNodeAY(float yPos) 
    { 
        if (!isSetup) SetupNodeLineDraw();
        Vector3 pos = nodes[0].transform.position; 
        nodes[0].transform.position = new Vector3(pos.x, yPos, pos.z);
    }

    public void SetNodeBX(float xPos) 
    { 
        if (!isSetup) SetupNodeLineDraw();
        Vector3 pos = nodes[1].transform.position; 
        nodes[1].transform.position = new Vector3(xPos, pos.y, pos.z);
    }

    public void SetNodeBY(float yPos) 
    { 
        if (!isSetup) SetupNodeLineDraw();
        Vector3 pos = nodes[1].transform.position; 
        nodes[1].transform.position = new Vector3(pos.x, yPos, pos.z);
    }
}
