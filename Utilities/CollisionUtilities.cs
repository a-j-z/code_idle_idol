using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionUtilities : MonoBehaviour
{
    public static bool GetCollision(GameObject go, Vector3 positionOffset, Vector2 overlapBoxDims, LayerMask layer, bool debug = false)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(go.transform.position + positionOffset, overlapBoxDims, 0, layer);
        
        
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != go &&
                !colliders[i].gameObject.transform.IsChildOf(go.transform))
            {
                if (debug) DebugDrawBox(go, positionOffset, overlapBoxDims, Color.green);
                return true;
            }
        }
        if (debug) DebugDrawBox(go, positionOffset, overlapBoxDims, Color.red);
        return false;
    }

    public static float GetCollisionDistance(GameObject go, Vector2 origin, Vector2 direction, float distance, LayerMask layer, bool debug = false)
    {
        Vector2 vec2pos = (Vector2)go.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(vec2pos + origin, direction, distance, layer);
        if (debug) Debug.DrawLine(vec2pos + origin, vec2pos + origin + (direction * distance), Color.red);

        float output;
        if (hit.collider != null)
        {
            output = Vector2.Distance(hit.point, vec2pos + origin);
        }
        else
        {
            output = distance;
        }
        if (debug) Debug.DrawLine(vec2pos + origin, vec2pos + origin + (direction * (output)), Color.green);
        return output;
    }

    public static void DebugDrawBox(GameObject go, Vector3 positionOffset, Vector2 overlapBoxDims, Color color)
    {
        Vector3 v1 = go.transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0);
        Vector3 v2 = go.transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0);
        Vector3 v3 = go.transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0);
        Vector3 v4 = go.transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0);

        Debug.DrawLine(v1, v2, color);
        Debug.DrawLine(v3, v4, color);
        Debug.DrawLine(v1, v3, color);
        Debug.DrawLine(v2, v4, color);
    }
}
