using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class PaletteDrag : MonoBehaviour
{
    private UnityAction<string, string> action;
    
    private RectTransform rt;
    private RectTransform parentRt;
    private RectTransform contentRt;

    private Vector2 startDragMouse;
    private Vector2 startDragButton;

    private Vector2 posChange;
    private bool isDragged;
    private string m_Name;
    private RectTransform moveLayerPreview;
    private LevelDraw draw;
    private int nButtons;

    public void SetValues(string name, RectTransform moveLayerPreview, LevelDraw draw)
    {
        rt = GetComponent<RectTransform>();
        parentRt = transform.parent.GetComponent<RectTransform>();
        contentRt = parentRt.transform.parent.GetComponent<RectTransform>();
        action = draw.MoveLayer;
        m_Name = name;
        this.moveLayerPreview = moveLayerPreview;
        this.draw = draw;
        nButtons = LevelParse.GetTileTypes().Length;
    }

    void Update()
    {
        if (MouseUtilities.TouchingUI(rt) && Input.GetMouseButtonDown(0))
        {
            startDragMouse = Input.mousePosition;
            startDragButton = parentRt.position;
            isDragged = true;
            moveLayerPreview.gameObject.SetActive(true);
        }
        if (Input.GetMouseButton(0) && isDragged)
        {
            posChange = (Vector2)Input.mousePosition - startDragMouse;
            parentRt.position = startDragButton + posChange;
            if (parentRt.position.x < contentRt.position.x + contentRt.rect.width / 2f - 10f)
            {
                parentRt.position = new Vector2(contentRt.position.x + contentRt.rect.width / 2f - 10f, parentRt.position.y);
            }
            else if (parentRt.position.x > contentRt.position.x + contentRt.rect.width / 2f + 10f)
            {
                parentRt.position = new Vector2(contentRt.position.x + contentRt.rect.width / 2f + 10f, parentRt.position.y);
            }
            if (parentRt.position.y < contentRt.position.y - 80.0f * nButtons - 5.0f)
            {
                parentRt.position = new Vector2(parentRt.position.x, contentRt.position.y - 80.0f * nButtons - 5.0f);
            }

            moveLayerPreview.position = new Vector3(
                contentRt.position.x - 25,
                contentRt.position.y - Mathf.Floor(
                    (contentRt.position.y - parentRt.position.y - 10.0f) / 80.0f + 1.0f) * 80.0f,
                0);
        }
        if (Input.GetMouseButtonUp(0) && isDragged)
        {
            action.Invoke(m_Name,
                draw.GetLayer(Mathf.Floor(
                    (contentRt.position.y - parentRt.position.y - 10.0f) / 80.0f)));
            isDragged = false;
            moveLayerPreview.gameObject.SetActive(false);
        }
    }

    public bool IsDragged()
    {
        return isDragged;
    }
}
