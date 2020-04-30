using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePicker : MonoBehaviour
{
    public static string GetTile(List<Vector3Int> tiles, Vector3Int location, string type)
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

        return output + "_" + type + "_1";
    }

    public static IList<Vector2[]> GeneratePhysicsShape(float size, Sprite sprite)
    {
        IList<Vector2[]> output = new List<Vector2[]>();
        Vector2 center = sprite.rect.size / 2f;
        float radius = center.x * size;
        string spriteName = sprite.name.Split('_')[0];

        Vector2[] boxInside = {
            new Vector2(center.x - radius, center.y - radius),
            new Vector2(center.x - radius, center.y + radius),
            new Vector2(center.x + radius, center.y + radius),
            new Vector2(center.x + radius, center.y - radius)
        }; output.Add(boxInside);

        if (spriteName.Contains("a"))
        {
            Vector2[] boxTopLeft = {
                new Vector2(0, sprite.rect.size.y),
                new Vector2(0, center.y + radius),
                new Vector2(center.x - radius, center.y + radius),
                new Vector2(center.x - radius, sprite.rect.size.y)
            }; output.Add(boxTopLeft);
        }

        if (spriteName.Contains("b"))
        {
            Vector2[] boxTop = {
                new Vector2(center.x - radius, sprite.rect.size.y),
                new Vector2(center.x - radius, center.y + radius),
                new Vector2(center.x + radius, center.y + radius),
                new Vector2(center.x + radius, sprite.rect.size.y)
            }; output.Add(boxTop);
        }

        if (spriteName.Contains("c"))
        {
            Vector2[] boxTopRight = {
                new Vector2(center.x + radius, sprite.rect.size.y),
                new Vector2(center.x + radius, center.y + radius),
                new Vector2(sprite.rect.size.x, center.y + radius),
                new Vector2(sprite.rect.size.x, sprite.rect.size.y)
            }; output.Add(boxTopRight);
        }

        if (spriteName.Contains("d"))
        {
            Vector2[] boxLeft = {
                new Vector2(0, center.y - radius),
                new Vector2(0, center.y + radius),
                new Vector2(center.x - radius, center.y + radius),
                new Vector2(center.x - radius, center.y - radius)
            }; output.Add(boxLeft);
        }

        if (spriteName.Contains("e"))
        {
            Vector2[] boxRight = {
                new Vector2(center.x + radius, center.y - radius),
                new Vector2(center.x + radius, center.y + radius),
                new Vector2(sprite.rect.size.x, center.y + radius),
                new Vector2(sprite.rect.size.x, center.y - radius)
            }; output.Add(boxRight);
        }

        if (spriteName.Contains("f"))
        {
            Vector2[] boxBottomLeft = {
                new Vector2(0, 0),
                new Vector2(0, center.y - radius),
                new Vector2(center.x - radius, center.y - radius),
                new Vector2(center.x - radius, 0)
            }; output.Add(boxBottomLeft);
        }

        if (spriteName.Contains("g"))
        {
            Vector2[] boxBottom = {
                new Vector2(center.x - radius, 0),
                new Vector2(center.x - radius, center.y - radius),
                new Vector2(center.x + radius, center.y - radius),
                new Vector2(center.x + radius, 0)
            }; output.Add(boxBottom);
        }

        if (spriteName.Contains("h"))
        {
            Vector2[] boxBottomRight = {
                new Vector2(center.x + radius, 0),
                new Vector2(center.x + radius, center.y - radius),
                new Vector2(sprite.rect.size.x, center.y - radius),
                new Vector2(sprite.rect.size.x, 0)
            }; output.Add(boxBottomRight);
        }

        return output;
    }
}
