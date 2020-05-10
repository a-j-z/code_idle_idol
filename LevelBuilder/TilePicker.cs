using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePicker : MonoBehaviour
{
    public static string GetTile(List<Vector3Int> tiles, Vector3Int location, string type, List<string> tileNames)
    {
        string output = "";

        /*
         A B C
         D x E
         F G H
         */

        //check A
        if (tiles.Contains(new Vector3Int(location.x - 1, location.y + 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)))
        { output += "a"; }

        //check B
        if (tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)))
        { output += "b"; }

        //check C
        if (tiles.Contains(new Vector3Int(location.x + 1, location.y + 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x, location.y + 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)))
        { output += "c"; }

        //check D
        if (tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)))
        { output += "d"; }

        //check E
        if (tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)))
        { output += "e"; }

        //check F
        if (tiles.Contains(new Vector3Int(location.x - 1, location.y - 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x - 1, location.y, 0)))
        { output += "f"; }

        //check G
        if (tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)))
        { output += "g"; }

        //check H
        if (tiles.Contains(new Vector3Int(location.x + 1, location.y - 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x, location.y - 1, 0)) &&
             tiles.Contains(new Vector3Int(location.x + 1, location.y, 0)))
        { output += "h"; }

        if (!tileNames.Contains(output + "_" + type + "_1"))
        {
            return "_" + type + "_1";
        }

        return output + "_" + type + "_1";
    }

    public static IList<Vector2[]> GeneratePhysicsShape(float size, Sprite sprite)
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

    public static void AngleSort(Vector2[] vertices, Vector2 center)
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

    public static float VectorAngle(Vector2 v, Vector2 center)
    {
        return Vector2.SignedAngle(new Vector2(0, 1), v - center);
    }
}
