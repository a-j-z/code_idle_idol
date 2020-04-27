using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Tools : int
{
    Erase = 1,
    Draw = 2
}

public class LevelDraw : MonoBehaviour
{
    public Camera cam;
    public RectTransform uiContainer;

    private Tilemap collidables;
    private Dictionary<Vector3Int, string> collidablesTiles;
    private Tools currentTool;

    void Start()
    {
        collidables = GameObject.FindGameObjectWithTag("collidables").GetComponent<Tilemap>();
        collidablesTiles = new Dictionary<Vector3Int, string>();
        currentTool = Tools.Draw;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !MouseUtilities.TouchingUI(cam, uiContainer.sizeDelta.y) && !PlayManager.GetIsPlay())
        {
            if (currentTool == Tools.Draw)
            {
                PlaceTile("Basic", MouseUtilities.GridSpace(cam));
            }
            else if (currentTool == Tools.Erase)
            {
                RemoveTile(MouseUtilities.GridSpace(cam));
            }
            DrawTiles();
        }
    }

    private void PlaceTile(string id, Vector3Int location)
    {
        if (!collidablesTiles.ContainsKey(location))
        {
            collidablesTiles.Add(location, id);
        }
    }

    private void RemoveTile(Vector3Int location)
    {
        if (collidablesTiles.ContainsKey(location))
        {
            collidablesTiles.Remove(location);
            collidables.SetTile(location, null);
        }
    }

    public void SetTool(int tool)
    {
        currentTool = (Tools)tool;
    }

    public int GetTool()
    {
        return (int)currentTool;
    }

    private void DrawTiles()
    {
        Tile tile;

        foreach (KeyValuePair<Vector3Int, string> entry in collidablesTiles)
        {
            tile = Resources.Load(GetTilePath(collidablesTiles, entry.Key, "Basic")) as Tile;
            collidables.SetTile(entry.Key, tile);
        }
    }

    public void Load()
    {
        string path = FileUtilities.LoadDialog();
        try
        {
            collidables.ClearAllTiles();
            collidablesTiles = LevelParse.ParseFile(path);
        }
        catch (NullReferenceException)
        {
            Debug.Log("Could not parse level file.");
            return;
        }
        DrawTiles();
    }

    public void Save()
    {
        string path = FileUtilities.SaveDialog();
        LevelParse.EncodeFile(collidablesTiles, path);
    }

    private string GetTilePath(Dictionary<Vector3Int, string> tiles, Vector3Int key, String type)
    {
        string output = "Tiles/Basic/";

        /*
         A B C
         D x E
         F G H
         */

        //check A
        if (
            tiles.ContainsKey(new Vector3Int(key.x - 1, key.y + 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x, key.y + 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x - 1, key.y, 0))
        ) {
            output += "a";
        }

        //check B
        if (
            tiles.ContainsKey(new Vector3Int(key.x, key.y + 1, 0))
        ) {
            output += "b";
        }

        //check C
        if (
            tiles.ContainsKey(new Vector3Int(key.x + 1, key.y + 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x, key.y + 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x + 1, key.y, 0))
        ) {
            output += "c";
        }

        //check D
        if (
            tiles.ContainsKey(new Vector3Int(key.x - 1, key.y, 0))
        ) {
            output += "d";
        }

        //check E
        if (
            tiles.ContainsKey(new Vector3Int(key.x + 1, key.y, 0))
        ) {
            output += "e";
        }

        //check F
        if (
            tiles.ContainsKey(new Vector3Int(key.x - 1, key.y - 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x, key.y - 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x - 1, key.y, 0))
        ) {
            output += "f";
        }

        //check G
        if (
            tiles.ContainsKey(new Vector3Int(key.x, key.y - 1, 0))
        ) {
            output += "g";
        }

        //check H
        if (
            tiles.ContainsKey(new Vector3Int(key.x + 1, key.y - 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x, key.y - 1, 0)) &&
            tiles.ContainsKey(new Vector3Int(key.x + 1, key.y, 0))
        ) {
            output += "h";
        }

        return output + "_1";
    }
}

/*
 TODO:
 Refactoring to deal with different types of tile generation:
 Spikes don't work like this.
 Also I won't make interactables tile maps.
     */