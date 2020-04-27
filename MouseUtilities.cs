using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseUtilities : MonoBehaviour
{
    public static Vector3 WorldSpace(Camera cam)
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
    
    public static Vector3Int GridSpace(Camera cam)
    {
        return new Vector3Int((int)Mathf.Round(WorldSpace(cam).x), (int)Mathf.Round(WorldSpace(cam).y), 0);
    }

    public static bool TouchingUI(Camera cam, float uiHeight)
    {
        return Input.mousePosition.y > Screen.height - uiHeight;
    }
}
