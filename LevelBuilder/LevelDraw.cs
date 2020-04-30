using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;

public enum Tools : int
{
    Rect = 1,
    Draw = 2
}

public class LevelDraw : MonoBehaviour
{
    public Camera cam;
    public RectTransform uiContainer;

    private string[] tileTypes;
    private Dictionary<string, Tilemap> tilemaps;
    private Dictionary<string, List<Vector3Int>> tiles;
    private Tools currentTool;

    private Dictionary<string, Tile> loadedTiles;

    private Vector3Int currMousePos = Vector3Int.zero;
    private Vector3Int prevMousePos = Vector3Int.zero;
    private GameObject rect;

    void Start()
    {
        tileTypes = LevelParse.GetTileTypes();
        loadedTiles = new Dictionary<string, Tile>();
        tilemaps = new Dictionary<string, Tilemap>();
        tiles = new Dictionary<string, List<Vector3Int>>();
        currentTool = Tools.Draw;
        rect = new GameObject("rect");
        rect.AddComponent<RectDraw>();

        LoadTiles();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !PlayManager.GetIsPlay())
        {
            if (currentTool == Tools.Draw)
            {
                currMousePos = MouseUtilities.GridSpace(cam);
                prevMousePos = currMousePos;
            }
            else if (currentTool == Tools.Rect)
            {
                prevMousePos = MouseUtilities.GridSpace(cam);
            }
        }
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !PlayManager.GetIsPlay())
        {
            bool draw;
            draw = Input.GetMouseButton(0);
            if (currentTool == Tools.Draw)
            {
                currMousePos = MouseUtilities.GridSpace(cam);
                DrawTileLine(tileTypes[0], currMousePos, prevMousePos, draw);
                prevMousePos = MouseUtilities.GridSpace(cam);
                DrawTiles();
            }
            else if (currentTool == Tools.Rect)
            {
                currMousePos = MouseUtilities.GridSpace(cam);
                rect.SetActive(true);
                rect.GetComponent<RectDraw>().Draw(currMousePos, prevMousePos);
            }
        }
        else
        {
            rect.SetActive(false);
        }
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !PlayManager.GetIsPlay())
        {
            bool draw;
            draw = Input.GetMouseButtonUp(0);
            if (currentTool == Tools.Rect)
            {
                DrawTileRect(tileTypes[0], currMousePos, prevMousePos, draw);
                DrawTiles();
            }
        }
    }

    private void LoadTiles()
    {
        Tile tile;
        Sprite tileSprite;
        char[] splitter = { '/', '\\' };

        string[] fileArray;

        string[] splitFileName;
        string fileName;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            fileArray = Directory.GetFiles(Application.dataPath + "/Resources/Tiles/" + tileTypes[i], "*.png", SearchOption.AllDirectories);

            for (int j = 0; j < fileArray.Length; j++)
            {
                splitFileName = fileArray[j].Split(splitter);
                fileName = splitFileName[splitFileName.Length - 1].Split('.')[0];
                tileSprite = Resources.Load<Sprite>("Tiles/" + tileTypes[i] + "/" + fileName);
                tileSprite.OverridePhysicsShape(TilePicker.GeneratePhysicsShape(0.6f, tileSprite));
                tile = ScriptableObject.CreateInstance("Tile") as Tile;
                tile.sprite = tileSprite;
                loadedTiles.Add(fileName, tile);
            }
        }

        GameObject tilemap = new GameObject();
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        tilemap.AddComponent<TilemapCollider2D>();
        tilemap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        tilemap.AddComponent<CompositeCollider2D>();
        tilemap.GetComponent<Rigidbody2D>().isKinematic = true;

        GameObject currentMap;
        
        for (int i = 0; i < tileTypes.Length; i++)
        {
            currentMap = Instantiate(tilemap, transform);
            currentMap.name = tileTypes[i];
            tilemaps.Add(tileTypes[i], currentMap.GetComponent<Tilemap>());
            tiles.Add(tileTypes[i], new List<Vector3Int>());
        }

        Destroy(tilemap);
    }

    private void DrawTileLine(string id, Vector3Int start, Vector3Int end, bool draw)
    {
        int xMin = Mathf.Min(start.x, end.x);
        int xMax = Mathf.Max(start.x, end.x);
        int yMin = Mathf.Min(start.y, end.y);
        int yMax = Mathf.Max(start.y, end.y);

        if (draw)
        {
            PlaceTile(id, start);
            PlaceTile(id, end);
        }
        else
        {
            RemoveTile(id, start);
            RemoveTile(id, end);
        }

        for (int x = xMin; x < xMax + 1; x++)
        {
            for (int y = yMin; y < yMax + 1; y++)
            {
                if (GeometryUtilities.LineRect(start.x + 0.5f, start.y + 0.5f, end.x + 0.5f, end.y + 0.5f, x, y, 1, 1))
                {
                    if (draw) PlaceTile(id, new Vector3Int(x, y, 0));
                    else RemoveTile(id, new Vector3Int(x, y, 0));
                }
            }
        }
    }

    private void DrawTileRect(string id, Vector3Int start, Vector3Int end, bool draw)
    {
        int xMin = Mathf.Min(start.x, end.x);
        int xMax = Mathf.Max(start.x, end.x);
        int yMin = Mathf.Min(start.y, end.y);
        int yMax = Mathf.Max(start.y, end.y);

        for (int x = xMin; x < xMax + 1; x++)
        {
            for (int y = yMin; y < yMax + 1; y++)
            {
                if (draw) PlaceTile(id, new Vector3Int(x, y, 0));
                else RemoveTile(id, new Vector3Int(x, y, 0));
            }
        }
    }

    private void PlaceTile(string id, Vector3Int location)
    {
        if (!tiles[id].Contains(location))
        {
            tiles[id].Add(location);
        }
    }

    private void RemoveTile(string id, Vector3Int location)
    {
        if (tiles[id].Contains(location))
        {
            tiles[id].Remove(location);

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
        foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
        {
            tilemaps[entry.Key].ClearAllTiles();
            foreach (Vector3Int location in entry.Value)
            {
                tilemaps[entry.Key].SetTile(location, loadedTiles[TilePicker.GetTile(tiles[entry.Key], location, entry.Key)]);
            }
        }
    }

    public void Load()
    {
        string path = FileUtilities.LoadDialog();
        try
        {
            foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
            {
                tilemaps[entry.Key].ClearAllTiles();
            }
            tiles = LevelParse.ParseFile(path);
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
        try
        {
            LevelParse.EncodeFile(tiles, path);
        }
        catch (ArgumentException)
        {
            Debug.Log("Could not save file.");
            return;
        }
    }
}