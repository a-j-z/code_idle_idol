using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryUtilities : MonoBehaviour
{
    public static bool LineRect(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh)
    {
        bool left = LineLine(x1, y1, x2, y2, rx, ry, rx, ry + rh);
        bool right = LineLine(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh);
        bool top = LineLine(x1, y1, x2, y2, rx, ry, rx + rw, ry);
        bool bottom = LineLine(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh);

        return left || right || top || bottom;

    }

    public static bool LineLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
        float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

        return uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1;
    }
}
