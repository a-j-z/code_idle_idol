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

public enum PaletteType : int
{
    Collidable = 1,
    Noncollidable = 2,
    Semisolid = 3,
    IdolFilter = 4
    //Danger = 5
}

public class LevelDraw : MonoBehaviour
{
    public Camera cam;
    public PaletteMenuManager paletteMenuManager;
    public BoxCollider2D playerSemisolidCollider;
    [SerializeField] private LayerMask SafeCollidableLayer = new LayerMask();
    [SerializeField] private LayerMask IdolFilterLayer = new LayerMask();

    private string[] tileTypes;
    private Dictionary<string, float> tileSizes;
    private Dictionary<string, Tilemap> tilemaps;
    private Dictionary<string, PaletteType> paletteTypes;
    private Dictionary<string, List<Vector3Int>> tiles;

    private Tools currentTool;
    private int currentPalette;

    private Dictionary<string, Tile> loadedTiles;
    private List<string> tileNames;

    private Vector3Int currMousePos = Vector3Int.zero;
    private Vector3Int prevMousePos = Vector3Int.zero;
    private GameObject rect;

    

    void Start()
    {
        tileTypes = LevelParse.GetTileTypes();
        loadedTiles = new Dictionary<string, Tile>();
        tileSizes = LevelParse.LoadTileSizes();
        tilemaps = new Dictionary<string, Tilemap>();
        paletteTypes = new Dictionary<string, PaletteType>();
        tiles = new Dictionary<string, List<Vector3Int>>();
        currentTool = Tools.Draw;
        currentPalette = 0;
        rect = new GameObject("rect");
        rect.AddComponent<RectDraw>();
        rect.transform.parent = transform;

        LoadTiles();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive())
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
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive())
        {
            bool draw;
            draw = Input.GetMouseButton(0);
            if (currentTool == Tools.Draw)
            {
                currMousePos = MouseUtilities.GridSpace(cam);
                DrawTileLine(tileTypes[currentPalette], currMousePos, prevMousePos, draw);
                prevMousePos = MouseUtilities.GridSpace(cam);
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
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive())
        {
            bool draw;
            draw = Input.GetMouseButtonUp(0);
            if (currentTool == Tools.Rect)
            {
                DrawTileRect(tileTypes[currentPalette], currMousePos, prevMousePos, draw);
            }
        }
        UpdatePalettes();
    }

    public void UpdatePalettes(string paletteName = "")
    {
        if (!paletteName.Equals(""))
        {
            paletteTypes[paletteName]++;
            if (paletteTypes[paletteName] > PaletteType.IdolFilter)
            {
                paletteTypes[paletteName] = PaletteType.Collidable;
            }
        }

        TilemapCollider2D collider;
        PlatformEffector2D effector;
        CompositeCollider2D colliderComposite;
        foreach (KeyValuePair<string, PaletteType> entry in paletteTypes)
        {
            collider = tilemaps[entry.Key].gameObject.GetComponent<TilemapCollider2D>();
            effector = tilemaps[entry.Key].gameObject.GetComponent<PlatformEffector2D>();
            colliderComposite = tilemaps[entry.Key].gameObject.GetComponent<CompositeCollider2D>();
            if (entry.Value == PaletteType.Collidable)
            {
                int[] layers = {8, 9};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(SafeCollidableLayer);
                collider.enabled = true;
                effector.surfaceArc = 360f;
            }
            else if (entry.Value == PaletteType.Noncollidable)
            {
                collider.enabled = false;
            }
            else if (entry.Value == PaletteType.Semisolid)
            {
                effector.surfaceArc = 1f;

                collider.enabled = true;
                Collider2D[] colliders = new Collider2D[tileTypes.Length];
                ContactFilter2D contactFilter = new ContactFilter2D();
                contactFilter = contactFilter.NoFilter();
                int overlapCount = Physics2D.OverlapCollider(playerSemisolidCollider, contactFilter, colliders);
                for (int i = 0; i < overlapCount; i++)
                {
                    if (colliders[i].gameObject.name.Equals(collider.gameObject.name))
                    {
                        collider.enabled = false;
                        break;
                    }
                }
            }
            else if (entry.Value == PaletteType.IdolFilter)
            {
                int[] layers = {9};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(IdolFilterLayer);
                effector.surfaceArc = 360f;
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
                tileSprite.OverridePhysicsShape(TilePicker.GeneratePhysicsShape(tileSizes[tileTypes[i]], tileSprite));
                tile = ScriptableObject.CreateInstance("Tile") as Tile;
                tile.sprite = tileSprite;
                loadedTiles.Add(fileName, tile);
            }
        }

        tileNames = new List<string>(loadedTiles.Keys);

        GameObject tilemap = new GameObject();
        tilemap.layer = LayerUtilities.LayerNumber(SafeCollidableLayer);
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        tilemap.AddComponent<TilemapCollider2D>();
        tilemap.AddComponent<CompositeCollider2D>();
        tilemap.AddComponent<PlatformEffector2D>();

        tilemap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        tilemap.GetComponent<PlatformEffector2D>().surfaceArc = 360f;
        tilemap.GetComponent<CompositeCollider2D>().usedByEffector = true;
        tilemap.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
        tilemap.GetComponent<Rigidbody2D>().isKinematic = true;

        GameObject currentMap;
        
        for (int i = 0; i < tileTypes.Length; i++)
        {
            currentMap = Instantiate(tilemap, transform);
            currentMap.name = tileTypes[i];
            currentMap.transform.position = Vector3.forward * i + new Vector3(-0.5f, -0.5f, 0);
            tilemaps.Add(tileTypes[i], currentMap.GetComponent<Tilemap>());
            paletteTypes.Add(tileTypes[i], PaletteType.Collidable);
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
            tilemaps[id].SetTile(location, loadedTiles[TilePicker.GetTile(tiles[id], location, id, tileNames)]);
            UpdateSurroundingTiles(location, id);
        }
    }

    private void RemoveTile(string id, Vector3Int location)
    {
        if (tiles[id].Contains(location))
        {
            tiles[id].Remove(location);
            tilemaps[id].SetTile(location, null);
            UpdateSurroundingTiles(location, id);
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

    public void SetPalette(int palette)
    {
        currentPalette = palette;
    }

    public int GetPalette()
    {
        return currentPalette;
    }

    public int GetPalette(string paletteName)
    {
        return (int)paletteTypes[paletteName];
    }

    public float GetLayer(string layer)
    {
        return tilemaps[layer].transform.position.z;
    }

    public string GetLayer(float z)
    {
        string output = "";
        foreach(KeyValuePair<string, Tilemap> entry in tilemaps)
        {
            if (entry.Value.transform.position.z == z)
            {
                output = entry.Key;
                break;
            }
        }
        return output;
    }
    
    public void MoveLayer(string layer, string behind)
    {
        float layerZ = tilemaps[layer].transform.position.z;
        float behindZ;
        if (tilemaps.ContainsKey(behind))
        {
            behindZ = tilemaps[behind].transform.position.z;
        }
        else behindZ = -1.0f;

        if (layerZ < behindZ)
        {
            foreach(KeyValuePair<string, Tilemap> entry in tilemaps)
            {
                if (entry.Value.transform.position.z <= behindZ &&
                    entry.Value.transform.position.z > layerZ)
                {
                    entry.Value.transform.position += Vector3.back;
                }
            }

            tilemaps[layer].transform.position =
                new Vector3(tilemaps[layer].transform.position.x,
                tilemaps[layer].transform.position.y,
                behindZ);
        }
        else if (layerZ > behindZ)
        {
            foreach (KeyValuePair<string, Tilemap> entry in tilemaps)
            {
                if (entry.Value.transform.position.z > behindZ &&
                    entry.Value.transform.position.z < layerZ)
                {
                    entry.Value.transform.position += Vector3.forward;
                }
            }

            tilemaps[layer].transform.position =
                new Vector3(tilemaps[layer].transform.position.x,
                tilemaps[layer].transform.position.y,
                behindZ + 1.0f);
        }
    }

    private void UpdateSurroundingTiles(Vector3Int location, string id)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (tilemaps[id].GetTile<Tile>(location + new Vector3Int(x, y, 0)) != null
                    && !(x == 0 && y == 0))
                {
                    tilemaps[id].SetTile(location + new Vector3Int(x, y, 0),
                        loadedTiles[TilePicker.GetTile(tiles[id], location + new Vector3Int(x, y, 0), id, tileNames)]);
                }
            }
        }
    }

    private void DrawTiles()
    {
        foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
        {
            tilemaps[entry.Key].ClearAllTiles();
            foreach (Vector3Int location in entry.Value)
            {
                tilemaps[entry.Key].SetTile(location, loadedTiles[TilePicker.GetTile(tiles[entry.Key], location, entry.Key, tileNames)]);
            }
        }
    }

    private Dictionary<string, int> CastPaletteTypesToInt()
    {
        Dictionary<string, int> output = new Dictionary<string, int>();
        foreach (KeyValuePair<string, PaletteType> entry in paletteTypes)
        {
            output.Add(entry.Key, (int)entry.Value);
        }
        return output;
    }

    private void CastIntToPaletteType(Dictionary<string, int> intPalettes)
    {
        paletteTypes.Clear();
        foreach (KeyValuePair<string, int> entry in intPalettes)
        {
            paletteTypes.Add(entry.Key, (PaletteType)entry.Value);
        }
    }

    private Dictionary<string, int> GetPaletteLayerOrder()
    {
        Dictionary<string, int> output = new Dictionary<string, int>();
        foreach (KeyValuePair<string, Tilemap> entry in tilemaps)
        {
            output.Add(entry.Key, Mathf.FloorToInt(entry.Value.transform.position.z));
        }
        return output;
    }

    private void SetPaletteLayerOrder(Dictionary<string, int> layerOrder)
    {
        Vector3 pos;
        foreach (KeyValuePair<string, int> entry in layerOrder)
        {
            pos = tilemaps[entry.Key].transform.position;
            tilemaps[entry.Key].transform.position = new Vector3(pos.x, pos.y, entry.Value);
        }
    }

    public void New()
    {
        foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
        {
            tilemaps[entry.Key].ClearAllTiles();
            tiles[entry.Key].Clear();
        }
        DrawTiles();
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
            CastIntToPaletteType(LevelParse.ParsePaletteInfo(path, ">"));
            SetPaletteLayerOrder(LevelParse.ParsePaletteInfo(path, "!"));

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
            LevelParse.EncodeFile(tiles, CastPaletteTypesToInt(), GetPaletteLayerOrder(), path);
        }
        catch (ArgumentException)
        {
            Debug.Log("Could not save file.");
            return;
        }
    }
}