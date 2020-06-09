using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePicker : MonoBehaviour
{
    public static string GetTile(
        List<Vector3Int> tiles, BoundsInt bounds, Dictionary<string, int> tileVariations, Dictionary<string, float> tileUpdateRadiuses,
        Vector3Int location, string type, List<string> tileNames, float extremesBias = 0.9f, float noise = 0.4f, float shift = 0.25f)
    {
        bounds.xMax -= 1;
        bounds.yMax -= 1;
        string output = "";

        /*
         A B C
         D x E
         F G H
         */

        //check A
        if ((tiles.Contains(new Vector3Int(location.x - 1, location.y + 1, 0)) || (location.x - 1 < bounds.xMin || location.y + 1 > bounds.yMax)) &&
             (tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)) || location.y + 1 > bounds.yMax) &&
             (tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)) || location.x - 1 < bounds.xMin))
        { output += "a"; }

        //check B
        if (tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)) || location.y + 1 > bounds.yMax)
        { output += "b"; }

        //check C
        if ((tiles.Contains(new Vector3Int(location.x + 1, location.y + 1, 0)) || (location.x + 1 > bounds.xMax || location.y + 1 > bounds.yMax)) &&
             (tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)) || location.y + 1 > bounds.yMax) &&
             (tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)) || location.x + 1 > bounds.xMax))
        { output += "c"; }

        //check D
        if (tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)) || location.x - 1 < bounds.xMin)
        { output += "d"; }

        //check E
        if (tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)) || location.x + 1 > bounds.xMax)
        { output += "e"; }

        //check F
        if ((tiles.Contains(new Vector3Int(location.x - 1, location.y - 1, 0)) || (location.x - 1 < bounds.xMin || location.y - 1 < bounds.yMin)) &&
             (tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)) || location.y - 1 < bounds.yMin) &&
             (tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)) || location.x - 1 < bounds.xMin))
        { output += "f"; }

        //check G
        if (tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)) || location.y - 1 < bounds.yMin)
        { output += "g"; }

        //check H
        if ((tiles.Contains(new Vector3Int(location.x + 1, location.y - 1, 0)) || (location.x + 1 > bounds.xMax || location.y - 1 < bounds.yMin)) &&
             (tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)) || location.y - 1 < bounds.yMin) &&
             (tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)) || location.x + 1 > bounds.xMax))
        { output += "h"; }

        int random = Random.Range(1, tileVariations[output + "_" + type] + 1);
        if (output.Equals("abcdefgh") && tileUpdateRadiuses[type] > 1.0f)
        {
            float edgeScore = EdgeScore(tiles, bounds, location, Mathf.FloorToInt(tileUpdateRadiuses[type]));
            edgeScore += Random.Range(-noise, noise) - ((extremesBias - 1) / 2f) + shift;
            edgeScore *= extremesBias;
            random = Mathf.RoundToInt(tileVariations[output + "_" + type] * edgeScore);
            if (random < 1) random = 1;
            else if (random > tileVariations[output + "_" + type]) random = tileVariations[output + "_" + type];
        }

        if (!tileNames.Contains(output + "_" + type + "_" + random))
        {
            return "_" + type + "_1";
        }

        return output + "_" + type + "_" + random;
    }

    public static IList<Vector2[]> GeneratePhysicsShape(float size, Sprite sprite, string generationType)
    {
        if (generationType.Equals("full"))
        {
            return GeneratePhysicsShapeFull(size, sprite);
        }
        else if (generationType.Equals("semisolid"))
        {
            return GeneratePhysicsShapeSemisolid(size, sprite);
        }
        else if (generationType.Equals("outline"))
        {
            return GeneratePhysicsShapeOutline(size, sprite);
        }
        return null;
    }
    
    private static IList<Vector2[]> GeneratePhysicsShapeFull(float size, Sprite sprite)
    {
        IList<Vector2[]> output = new List<Vector2[]>();
        Vector2 center = sprite.rect.size / 2f;
        float radius = center.x * size;
        string spriteName = sprite.name.Split('_')[0];

        List<Vector2> vertices = new List<Vector2>();

        vertices.Add(new Vector2(center.x - radius, center.y - radius));
        vertices.Add(new Vector2(center.x - radius, center.y + radius));
        vertices.Add(new Vector2(center.x + radius, center.y + radius));
        vertices.Add(new Vector2(center.x + radius, center.y - radius));

        if (spriteName.Contains("a"))
        {
            vertices.Add(new Vector2(0, sprite.rect.size.y));
            vertices.Add(new Vector2(0, center.y + radius));
            vertices.Add(new Vector2(center.x - radius, sprite.rect.size.y));
            
        }

        if (spriteName.Contains("b"))
        {
            vertices.Add(new Vector2(center.x - radius, sprite.rect.size.y));
            vertices.Add(new Vector2(center.x - radius, center.y + radius));
            vertices.Add(new Vector2(center.x + radius, center.y + radius));
            vertices.Add(new Vector2(center.x + radius, sprite.rect.size.y));
        }

        if (spriteName.Contains("c"))
        {
            vertices.Add(new Vector2(center.x + radius, sprite.rect.size.y));
            vertices.Add(new Vector2(sprite.rect.size.x, center.y + radius));
            vertices.Add(new Vector2(sprite.rect.size.x, sprite.rect.size.y));
        }

        if (spriteName.Contains("d"))
        {
            vertices.Add(new Vector2(0, center.y - radius));
            vertices.Add(new Vector2(0, center.y + radius));
            vertices.Add(new Vector2(center.x - radius, center.y + radius));
            vertices.Add(new Vector2(center.x - radius, center.y - radius));
        }

        if (spriteName.Contains("e"))
        {
            vertices.Add(new Vector2(center.x + radius, center.y - radius));
            vertices.Add(new Vector2(center.x + radius, center.y + radius));
            vertices.Add(new Vector2(sprite.rect.size.x, center.y + radius));
            vertices.Add(new Vector2(sprite.rect.size.x, center.y - radius));
        }

        if (spriteName.Contains("f"))
        {
            vertices.Add(new Vector2(0, 0));
            vertices.Add(new Vector2(0, center.y - radius));
            vertices.Add(new Vector2(center.x - radius, 0));
        }

        if (spriteName.Contains("g"))
        {
            vertices.Add(new Vector2(center.x - radius, 0));
            vertices.Add(new Vector2(center.x - radius, center.y - radius));
            vertices.Add(new Vector2(center.x + radius, center.y - radius));
            vertices.Add(new Vector2(center.x + radius, 0));
        }

        if (spriteName.Contains("h"))
        {
            vertices.Add(new Vector2(center.x + radius, 0));
            vertices.Add(new Vector2(sprite.rect.size.x, center.y - radius));
            vertices.Add(new Vector2(sprite.rect.size.x, 0));
        }

        List<Vector2> verticesUnique = vertices.Distinct().ToList();

        if (spriteName.Contains("a")) verticesUnique.Remove(new Vector2(center.x - radius, center.y + radius));
        if (spriteName.Contains("c")) verticesUnique.Remove(new Vector2(center.x + radius, center.y + radius));
        if (spriteName.Contains("f")) verticesUnique.Remove(new Vector2(center.x - radius, center.y - radius));
        if (spriteName.Contains("h")) verticesUnique.Remove(new Vector2(center.x + radius, center.y - radius));

        Vector2[] uniqueArray = verticesUnique.ToArray();
        AngleSort(uniqueArray, center);
        output.Add(uniqueArray);

        return output;
    }

    private static IList<Vector2[]> GeneratePhysicsShapeSemisolid(float size, Sprite sprite)
    {
        IList<Vector2[]> output = new List<Vector2[]>();
        Vector2 center = sprite.rect.size / 2f;
        float radius = center.x * size;
        string spriteName = sprite.name.Split('_')[0];

        bool left = !spriteName.Contains("a") && spriteName.Contains("d");
        bool middle = !spriteName.Contains("b");
        bool right = !spriteName.Contains("c") && spriteName.Contains("e");

        Vector2 pointA = new Vector2(0, center.y + radius);
        Vector2 pointB = new Vector2(center.x - radius, center.y + radius);
        Vector2 pointC = new Vector2(center.x + radius, center.y + radius);
        Vector2 pointD = new Vector2(sprite.rect.size.x, center.y + radius);
        Vector2 pointE = new Vector2(0, center.y + radius - 1f);
        Vector2 pointF = new Vector2(center.x - radius, center.y + radius - 1f);
        Vector2 pointG = new Vector2(center.x + radius, center.y + radius - 1f);
        Vector2 pointH = new Vector2(sprite.rect.size.x, center.y + radius - 1f);


        List<Vector2> vertices = new List<Vector2>();
        if (left && !middle && !right)
        {
            vertices.Add(pointA);
            vertices.Add(pointB);
            vertices.Add(pointF);
            vertices.Add(pointE);
            output.Add(vertices.ToArray());
        }
        else if (left && middle && !right)
        {
            vertices.Add(pointA);
            vertices.Add(pointC);
            vertices.Add(pointG);
            vertices.Add(pointE);
            output.Add(vertices.ToArray());
        }
        else if (left && middle && right)
        {
            vertices.Add(pointA);
            vertices.Add(pointD);
            vertices.Add(pointH);
            vertices.Add(pointE);
            output.Add(vertices.ToArray());
        }
        else if (!left && middle && right)
        {
            vertices.Add(pointB);
            vertices.Add(pointD);
            vertices.Add(pointH);
            vertices.Add(pointF);
            output.Add(vertices.ToArray());
        }
        else if (!left && !middle && right)
        {
            vertices.Add(pointC);
            vertices.Add(pointD);
            vertices.Add(pointH);
            vertices.Add(pointG);
            output.Add(vertices.ToArray());
        }
        else if (left && !middle && right)
        {
            vertices.Add(pointA);
            vertices.Add(pointB);
            vertices.Add(pointF);
            vertices.Add(pointE);
            output.Add(vertices.ToArray());
            vertices.Clear();
            vertices.Add(pointC);
            vertices.Add(pointD);
            vertices.Add(pointH);
            vertices.Add(pointG);
            output.Add(vertices.ToArray());
        }
        else if (!left && middle && !right)
        {
            vertices.Add(pointB);
            vertices.Add(pointC);
            vertices.Add(pointG);
            vertices.Add(pointF);
            output.Add(vertices.ToArray());
        }
        else 
        {
            vertices.Add(center);
            vertices.Add(center);
            vertices.Add(center);
            vertices.Add(center);
            output.Add(vertices.ToArray());
        }

        return output;
    }

    private static IList<Vector2[]> GeneratePhysicsShapeOutline(float size, Sprite sprite)
    {
        return null;
    }

    private static void AngleSort(Vector2[] vertices, Vector2 center)
    {
        int n = vertices.Length;
        for (int i = 1; i < n; ++i)
        {
            Vector2 key = vertices[i];
            int j = i - 1;

            while (j >= 0 && VectorAngle(vertices[j], center) > VectorAngle(key, center))
            {
                vertices[j + 1] = vertices[j];
                j = j - 1;
            }
            vertices[j + 1] = key;
        }
    }

    private static float VectorAngle(Vector2 v, Vector2 center)
    {
        return Vector2.SignedAngle(new Vector2(0, 1), v - center);
    }

    private static float EdgeScore(List<Vector3Int> tiles, BoundsInt bounds, Vector3Int location, int radius)
    {
        int closest = radius + 1;
        if (radius == 1) return 1;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (Mathf.Abs(x) > 1 || Mathf.Abs(y) > 1)
                {
                    if (!tiles.Contains(location + new Vector3Int(x, y, 0))
                        && (location + new Vector3Int(x, y, 0)).x < bounds.xMax && (location + new Vector3Int(x, y, 0)).y < bounds.yMax
                        && (location + new Vector3Int(x, y, 0)).x > bounds.xMin && (location + new Vector3Int(x, y, 0)).y > bounds.yMin)
                    {
                        int absX = Mathf.Abs(x); int absY = Mathf.Abs(y);
                        int maxXY = Mathf.Max(absX, absY);
                        if (closest > maxXY) closest = maxXY;
                    }
                }
            }
        }
        return ((float)(closest - (radius - 1))) / ((float)(radius - 1));
    }

    private static float EdgeScore2(List<Vector3Int> tiles, BoundsInt bounds, Vector3Int location, int radius)
    {
        int closest = radius + 1;
        if (radius <= 1) return 1;
        for (int r = 2; r <= radius; r++)
        {
            if (!tiles.Contains(location + new Vector3Int(r, 0, 0)) && (location + new Vector3Int(r, 0, 0)).x < bounds.xMax) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(-r, 0, 0)) && (location + new Vector3Int(-r, 0, 0)).x > bounds.xMin) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(0, r, 0)) && (location + new Vector3Int(0, r, 0)).y < bounds.yMax) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(0, -r, 0)) && (location + new Vector3Int(0, -r, 0)).y > bounds.yMin) { closest = r; break; }

            else if (!tiles.Contains(location + new Vector3Int(r, r, 0))
                && (location + new Vector3Int(r, 0, 0)).x < bounds.xMax && (location + new Vector3Int(0, r, 0)).y < bounds.yMax) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(r, -r, 0))
                && (location + new Vector3Int(r, 0, 0)).x < bounds.xMax && (location + new Vector3Int(0, -r, 0)).y > bounds.yMin) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(-r, r, 0))
                && (location + new Vector3Int(-r, 0, 0)).x > bounds.xMin && (location + new Vector3Int(0, r, 0)).y < bounds.yMax) { closest = r; break; }
            else if (!tiles.Contains(location + new Vector3Int(-r, -r, 0))
                && (location + new Vector3Int(-r, 0, 0)).x > bounds.xMin && (location + new Vector3Int(0, -r, 0)).y > bounds.yMin) { closest = r; break; }
        }
        return ((float)(closest - (radius - 1))) / ((float)(radius - 1));
    }
}
