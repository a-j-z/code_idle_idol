using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionUtilities : MonoBehaviour
{
    public static bool GetCollision(GameObject go, Vector3 positionOffset, Vector2 overlapBoxDims, LayerMask layer, bool debug = false)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(go.transform.position + positionOffset, overlapBoxDims, 0, layer);
        if (debug) DebugDrawBox(go, positionOffset, overlapBoxDims);
        
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != go &&
                !colliders[i].gameObject.transform.IsChildOf(go.transform))
            {
                return true;
            }
        }
        return false;
    }

    public static float GetCollisionDistance(GameObject go, Vector2 origin, Vector2 direction, float distance)
    {
        Vector2 vec2pos = (Vector2)go.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(vec2pos + origin, direction, distance);

        if (hit.collider != null)
        {
            return distance - Vector2.Distance(hit.point, vec2pos + origin);
        }
        return distance;
    }

    public static void DebugDrawBox(GameObject go, Vector3 positionOffset, Vector2 overlapBoxDims)
    {
        Vector3 v1 = go.transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0);
        Vector3 v2 = go.transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, overlapBoxDims.y * 0.5f, 0);
        Vector3 v3 = go.transform.position + positionOffset + new Vector3(-overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0);
        Vector3 v4 = go.transform.position + positionOffset + new Vector3(overlapBoxDims.x * 0.5f, -overlapBoxDims.y * 0.5f, 0);

        Debug.DrawLine(v1, v2, Color.red);
        Debug.DrawLine(v3, v4, Color.red);
        Debug.DrawLine(v1, v3, Color.red);
        Debug.DrawLine(v2, v4, Color.red);
    }
}
