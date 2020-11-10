using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
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
    IdolFilter = 4,
    Danger = 5
}

public class LevelDraw : MonoBehaviour
{
    public Camera buildCam;
    public Camera playCam;
    public GameObject player;
    public SpawnController spawns;
    public ExitController exits;
    public GateController gates;
    public PaletteMenuManager paletteMenuManager;
    public ObjectManager objectManager;
    [SerializeField] private LayerMask SafeCollidableLayer = new LayerMask();
    [SerializeField] private LayerMask IdolFilterLayer = new LayerMask();
    [SerializeField] private LayerMask SemisolidCollidableLayer = new LayerMask();
    [SerializeField] private LayerMask DangerCollidableLayer = new LayerMask();

    private string[] tileTypes;
    private Dictionary<string, float> tileSizes;
    private Dictionary<string, float> tileUpdateRadiuses;
    private Dictionary<string, Tilemap> tilemaps;
    private Dictionary<string, GameObject> layers;
    private Dictionary<string, PaletteType> paletteTypes;
    private Dictionary<string, List<Vector3Int>> tiles;
    private BoundsInt totalBounds;

    private Tools currentTool;
    private int currentPalette;
    private string currentLayer;

    private Dictionary<string, Tile> loadedTilesFull;
    private Dictionary<string, Tile> loadedTilesSemisolid;
    private Dictionary<string, Tile> loadedTilesOutline;
    private Dictionary<string, int> tileVariations;
    private List<string> tileNames;

    private Vector3Int currMousePos = Vector3Int.zero;
    private Vector3Int prevMousePos = Vector3Int.zero;
    private GameObject rect;
    private GameObject camBoundsRect;

    void Start()
    {
        tileTypes = LevelParse.GetTileTypes();
        loadedTilesFull = new Dictionary<string, Tile>();
        loadedTilesSemisolid = new Dictionary<string, Tile>();
        loadedTilesOutline = new Dictionary<string, Tile>();
        tileVariations = LevelParse.GetTileVariations();
        tileSizes = LevelParse.LoadTileData("sizes");
        tileUpdateRadiuses = LevelParse.LoadTileData("update_radiuses");
        tilemaps = new Dictionary<string, Tilemap>();
        layers = new Dictionary<string, GameObject>();
        paletteTypes = new Dictionary<string, PaletteType>();
        tiles = new Dictionary<string, List<Vector3Int>>();
        currentTool = Tools.Draw;
        currentPalette = 0;
        currentLayer = "Player";
        rect = new GameObject("Rect");
        rect.AddComponent<RectDraw>();
        rect.transform.parent = transform;
        camBoundsRect = new GameObject("CamBoundsRect");
        camBoundsRect.AddComponent<RectDraw>();
        camBoundsRect.transform.parent = transform;

        LoadTiles();
        New();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive() && currentPalette != -1)
        {
            if (currentTool == Tools.Draw)
            {
                currMousePos = MouseUtilities.GridSpace(buildCam);
                prevMousePos = currMousePos;
            }
            else if (currentTool == Tools.Rect)
            {
                prevMousePos = MouseUtilities.GridSpace(buildCam);
            }
        }
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive() && currentPalette != -1)
        {
            bool draw;
            draw = Input.GetMouseButton(0);
            if (currentTool == Tools.Draw)
            {
                currMousePos = MouseUtilities.GridSpace(buildCam);
                DrawTileLine(tileTypes[currentPalette], currMousePos, prevMousePos, draw);
                prevMousePos = MouseUtilities.GridSpace(buildCam);
            }
            else if (currentTool == Tools.Rect)
            {
                currMousePos = MouseUtilities.GridSpace(buildCam);
                rect.SetActive(true);
                rect.GetComponent<RectDraw>().Draw(currMousePos, prevMousePos);
            }
        }
        else
        {
            rect.SetActive(false);
        }
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !PlayManager.GetIsPlay()
            && !paletteMenuManager.IsPaletteMenuActive() && currentPalette != -1)
        {
            bool draw;
            draw = Input.GetMouseButtonUp(0);
            if (currentTool == Tools.Rect)
            {
                DrawTileRect(tileTypes[currentPalette], currMousePos, prevMousePos, draw);
            }
        }
        if (PlayManager.GetIsPlay()) { camBoundsRect.SetActive(false); }
        else { camBoundsRect.SetActive(true); }
    }

    public void UpdatePalettes(string paletteName = "")
    {
        if (!paletteName.Equals(""))
        {
            paletteTypes[paletteName]++;
            if (paletteTypes[paletteName] > PaletteType.Danger)
            {
                paletteTypes[paletteName] = PaletteType.Collidable;
            }
        }

        TilemapCollider2D collider;
        PlatformEffector2D effector;
        foreach (KeyValuePair<string, PaletteType> entry in paletteTypes)
        {
            collider = tilemaps[entry.Key].gameObject.GetComponent<TilemapCollider2D>();
            effector = tilemaps[entry.Key].gameObject.GetComponent<PlatformEffector2D>();
            if (entry.Value == PaletteType.Collidable)
            {
                int[] layers = {8, 9, 14};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                tilemaps[entry.Key].GetComponent<CompositeCollider2D>().isTrigger = false;
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(SafeCollidableLayer);
                SwitchTilePhysicsShape(entry.Key, "full");
                collider.enabled = true;
                effector.surfaceArc = 360f;
            }
            else if (entry.Value == PaletteType.Noncollidable)
            {
                collider.enabled = false;
            }
            else if (entry.Value == PaletteType.Semisolid)
            {
                int[] layers = {8, 9};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                effector.surfaceArc = 1f;
                collider.enabled = true;
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(SemisolidCollidableLayer);
                SwitchTilePhysicsShape(entry.Key, "semisolid");
            }
            else if (entry.Value == PaletteType.IdolFilter)
            {
                int[] layers = {9, 14};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(IdolFilterLayer);
                SwitchTilePhysicsShape(entry.Key, "full");
                effector.surfaceArc = 360f;
            }
            else if (entry.Value == PaletteType.Danger)
            {
                int[] layers = {8, 9, 14};
                tilemaps[entry.Key].GetComponent<PlatformEffector2D>().colliderMask = LayerUtilities.LayerNumbersToMask(layers);
                tilemaps[entry.Key].GetComponent<CompositeCollider2D>().isTrigger = true;
                tilemaps[entry.Key].gameObject.layer = LayerUtilities.LayerNumber(DangerCollidableLayer);
                SwitchTilePhysicsShape(entry.Key, "outline");
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
                string tempName = tileSprite.name;
                float spriteWidth = tileSprite.rect.width;
                float spriteHeight = tileSprite.rect.height;
                tileSprite = Sprite.Create(tileSprite.texture, tileSprite.rect,
                    new Vector2(((tileSprite.pivot.x + ((spriteWidth % 200 == 0) ? -50 : 0)) / spriteWidth), 
                    ((tileSprite.pivot.y + ((spriteHeight % 200 == 0) ? -50 : 0)) / spriteHeight)));
                tileSprite.name = tempName;

                tileSprite.OverridePhysicsShape(TilePicker.GeneratePhysicsShape(tileSizes[tileTypes[i]], tileSprite, "full"));
                tile = ScriptableObject.CreateInstance("Tile") as Tile;
                tile.sprite = Instantiate(tileSprite);
                loadedTilesFull.Add(fileName, Instantiate(tile));
                
                tileSprite.OverridePhysicsShape(TilePicker.GeneratePhysicsShape(tileSizes[tileTypes[i]], tileSprite, "semisolid"));
                tile = ScriptableObject.CreateInstance("Tile") as Tile;
                tile.sprite = Instantiate(tileSprite);
                loadedTilesSemisolid.Add(fileName, Instantiate(tile));

                tileSprite.OverridePhysicsShape(TilePicker.GeneratePhysicsShape(tileSizes[tileTypes[i]], tileSprite, "outline"));
                tile = ScriptableObject.CreateInstance("Tile") as Tile;
                tile.sprite = Instantiate(tileSprite);
                loadedTilesOutline.Add(fileName, Instantiate(tile));
            }
        }

        tileNames = new List<string>(loadedTilesFull.Keys);

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
        
        player.transform.position = Vector3.zero;
        layers.Add("Player", player);
        for (int i = 0; i < tileTypes.Length; i++)
        {
            currentMap = Instantiate(tilemap, transform);
            currentMap.name = tileTypes[i];
            currentMap.transform.position = Vector3.forward * (i + 1) + new Vector3(-0.5f, -0.5f, 0);
            tilemaps.Add(tileTypes[i], currentMap.GetComponent<Tilemap>());
            layers.Add(tileTypes[i], currentMap);
            //tilemapPhysicsShapes.Add(tileTypes[i], "full");
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
        if (tiles[id].Contains(location)) return;
        Dictionary<string, Tile> loadedTiles;
        if (paletteTypes[id] == PaletteType.Semisolid) loadedTiles = loadedTilesSemisolid;
        if (paletteTypes[id] == PaletteType.Danger) loadedTiles = loadedTilesOutline;
        else loadedTiles = loadedTilesFull;
        tiles[id].Add(location);
        tilemaps[id].SetTile(location, loadedTiles[TilePicker.GetTile(tiles[id], totalBounds, tileVariations, tileUpdateRadiuses, location, id, tileNames)]);
        playCam.GetComponent<PlayCameraController>().UpdateBounds(CalculateBounds());
        UpdateSurroundingTiles(location, id);
    }

    private void RemoveTile(string id, Vector3Int location)
    {
        if (!tiles[id].Contains(location)) return;
        tiles[id].Remove(location);
        tilemaps[id].SetTile(location, null);
        tilemaps[id].CompressBounds();
        playCam.GetComponent<PlayCameraController>().UpdateBounds(CalculateBounds());
        UpdateSurroundingTiles(location, id);
    }

    private BoundsInt CalculateBounds()
    {
        int xMin = 0, yMin = 0, xMax = 0, yMax = 0;
        foreach(KeyValuePair<string, Tilemap> entry in tilemaps)
        {
            BoundsInt currBounds = entry.Value.cellBounds;
            if (!currBounds.size.Equals(new Vector3Int(0, 0, 1))) 
            {
                if (currBounds.xMin < xMin) xMin = currBounds.xMin;
                if (currBounds.yMin < yMin) yMin = currBounds.yMin;
                if (currBounds.xMax > xMax) xMax = currBounds.xMax;
                if (currBounds.yMax > yMax) yMax = currBounds.yMax;
            }
        }

        int i = 0;
        while (xMax - xMin < 34)
        {
            if (i % 2 == 0) xMax++;
            else xMin--;
            i++; 
        }
        i = 0;
        while (yMax - yMin < 17)
        {
            if (i % 2 == 0) yMax++;
            else yMin--;
            i++; 
        }

        camBoundsRect.GetComponent<RectDraw>().Draw(
            new Vector3(xMin + 0.5f, yMin + 0.5f, 0),
            new Vector3(xMax - 1.5f, yMax - 1.5f, 0));

        BoundsInt output = new BoundsInt(xMin, yMin, 0, xMax - xMin, yMax - yMin, 0);
        Vector3[] spawnLocations = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        for (int spawn = 0; spawn < spawnLocations.Length; spawn++) 
        {
            Vector3[] spawnData = GetSpawn(spawnLocations[spawn], output);
            spawns.UpdatePosition(spawnLocations[spawn], spawnData[0]);
            exits.UpdatePosition(spawnLocations[spawn], GetExitPosition(spawnLocations[spawn], output));
            exits.UpdateScale(spawnLocations[spawn], GetExitScale(spawnLocations[spawn], output));
            gates.UpdatePosition(spawnLocations[spawn], spawnData[0]);
            gates.UpdateLength(spawnLocations[spawn], spawnData[1]);
        }

        if (totalBounds != output)
        {
            totalBounds = output;
            UpdateAllTiles();
        }
        return output;
    }

    private Vector3[] GetSpawn(Vector3 direction, BoundsInt bounds)
    {
        Vector3[] output = {Vector3.zero, Vector3.zero};
        Vector3[] defaultOutput = {Vector3.zero, Vector3.zero};
        int boundsMin = 0;
        int boundsMax = 0;
        int boundsConstant = 0;
        bool isVertical = true;
        if (direction.Equals(Vector3.up))
        {
            boundsMin = bounds.xMin;
            boundsMax = bounds.xMax - 1;
            boundsConstant = bounds.yMax - 1;
            isVertical = false;
        }
        else if (direction.Equals(Vector3.down))
        {
            boundsMin = bounds.xMin;
            boundsMax = bounds.xMax - 1;
            boundsConstant = bounds.yMin;
            isVertical = false;
        }
        else if (direction.Equals(Vector3.left))
        {
            boundsMin = bounds.yMin;
            boundsMax = bounds.yMax - 1;
            boundsConstant = bounds.xMin;
            isVertical = true;
        }
        else if (direction.Equals(Vector3.right))
        {
            boundsMin = bounds.yMin;
            boundsMax = bounds.yMax - 1;
            boundsConstant = bounds.xMax - 1;
            isVertical = true;
        }
        else return output;

        bool foundOpening = false;
        output[0] = new Vector3Int(boundsMin, boundsConstant, 0);
        int gapSize = 0;
        for (int i = boundsMin; i <= boundsMax; i++)
        {
            bool isTileHere = false;
            int x = isVertical ? boundsConstant : i;
            int y = isVertical ? i : boundsConstant;
            foreach (KeyValuePair<string, Tilemap> entry in tilemaps)
            {
                if (paletteTypes[entry.Key] == PaletteType.Collidable &&
                    entry.Value.GetTile(new Vector3Int(x, y, 0)))
                {
                    isTileHere = true; 
                    break;
                }
            }
            if (!isTileHere)
            {
                if (gapSize == 0) output[0] = new Vector3Int(x, y, 0);
                foundOpening = true;
                gapSize++;
            }
            if (isTileHere && gapSize > 0)
            {
                break;
            }
        }
        output[1] = new Vector3(gapSize, gapSize, gapSize);
        if (!foundOpening) 
        {
            return defaultOutput;
        }
        return output;
    }

    private Vector3 GetExitPosition(Vector3 direction, BoundsInt bounds)
    {
        Vector3 output = Vector3.zero;

        if (direction.Equals(Vector3.up))
        {
            output = new Vector3((float)(bounds.xMax + bounds.xMin) / 2.0f - 0.5f, bounds.yMax + 0.8f, 0f);
        }
        else if (direction.Equals(Vector3.down))
        {
            output = new Vector3((float)(bounds.xMax + bounds.xMin) / 2.0f - 0.5f, bounds.yMin - 1.8f, 0f);
        }
        else if (direction.Equals(Vector3.left))
        {
            output = new Vector3(bounds.xMin - 1.5f, (float)(bounds.yMax + bounds.yMin) / 2.0f - 0.5f, 0f);
        }
        else if (direction.Equals(Vector3.right))
        {
            output = new Vector3(bounds.xMax + 0.5f, (float)(bounds.yMax + bounds.yMin) / 2.0f - 0.5f, 0f);
        }

        return output;
    }

    private Vector3 GetExitScale(Vector3 direction, BoundsInt bounds)
    {
        Vector3 output = Vector3.zero;

        if (direction.Equals(Vector3.up) || direction.Equals(Vector3.down))
        {
            output = new Vector3(bounds.xMax - bounds.xMin, 1.0f, 0f);
        }
        else if (direction.Equals(Vector3.left) || direction.Equals(Vector3.right))
        {
            output = new Vector3(1.0f, bounds.yMax - bounds.yMin, 0f);
        }

        return output;
    }

    private void SwitchTilePhysicsShape(string id, string generationType)
    {
        //generationType = [full, semisolid, outline]
        BoundsInt bounds = tilemaps[id].cellBounds;
        
        Dictionary<string, Tile> currentTiles;
        if (generationType.Equals("full"))
        {
            currentTiles = loadedTilesFull;
        }
        else if (generationType.Equals("semisolid"))
        {
            currentTiles = loadedTilesSemisolid;
        }
        else if (generationType.Equals("outline"))
        {
            currentTiles = loadedTilesOutline;
        }
        else return;
        
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Tile tile = (Tile)tilemaps[id].GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    string name = tile.sprite.name.Substring(0, tile.sprite.name.Length - 7);
                    tilemaps[id].SetTile(new Vector3Int(x, y, 0), currentTiles[name]);
                    tilemaps[id].RefreshTile(new Vector3Int(x, y, 0));
                }
            }
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

    public void SetLayer(string layer)
    {
        currentLayer = layer;
        SetPalette(-1);
        for (int i = 0; i < tileTypes.Length; i++)
        {
            if (tileTypes[i] == layer)
            {
                SetPalette(i);
                break;
            }
        }
    }

    public int GetPalette()
    {
        return currentPalette;
    }

    public int GetPalette(string paletteName)
    {
        if (!paletteTypes.ContainsKey(paletteName))
            return -1;
        return (int)paletteTypes[paletteName];
    }

    public Vector2 GetCurrentPaletteSize()
    {
        if (currentPalette == -1) return Vector2.zero;
        return loadedTilesFull["_" + tileTypes[currentPalette] + "_1"].sprite.rect.size;// /
            //loadedTilesFull["_" + tileTypes[currentPalette] + "_1"].sprite.pixelsPerUnit;
    }

    public float GetLayerIndex(string layer)
    {
        return layers[layer].transform.position.z;
    }

    public string GetLayer()
    {
        return currentLayer;
    }

    public string GetLayer(float z)
    {
        string output = "";
        foreach(KeyValuePair<string, GameObject> entry in layers)
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
        float layerZ = layers[layer].transform.position.z;
        float behindZ;
        if (tilemaps.ContainsKey(behind))
        {
            behindZ = tilemaps[behind].transform.position.z;
        }
        else behindZ = -1.0f;

        if (layerZ < behindZ)
        {
            foreach(KeyValuePair<string, GameObject> entry in layers)
            {
                if (entry.Value.transform.position.z <= behindZ &&
                    entry.Value.transform.position.z > layerZ)
                {
                    entry.Value.transform.position += Vector3.back;
                }
            }

            layers[layer].transform.position =
                new Vector3(layers[layer].transform.position.x,
                layers[layer].transform.position.y,
                behindZ);
        }
        else if (layerZ > behindZ)
        {
            foreach (KeyValuePair<string, GameObject> entry in layers)
            {
                if (entry.Value.transform.position.z > behindZ &&
                    entry.Value.transform.position.z < layerZ)
                {
                    entry.Value.transform.position += Vector3.forward;
                }
            }

            layers[layer].transform.position =
                new Vector3(layers[layer].transform.position.x,
                layers[layer].transform.position.y,
                behindZ + 1.0f);
        }
    }

    public void AddLayer(string layer, GameObject newObject)
    {
        foreach(KeyValuePair<string, GameObject> entry in layers)
        {
            entry.Value.transform.position += Vector3.forward;
        }
        newObject.transform.position = Vector3.zero;
        layers[layer] = newObject;
    }

    private void UpdateSurroundingTiles(Vector3Int location, string id)
    {
        Dictionary<string, Tile> loadedTiles;
        if (paletteTypes[id] == PaletteType.Semisolid) loadedTiles = loadedTilesSemisolid;
        else if (paletteTypes[id] == PaletteType.Danger) loadedTiles = loadedTilesOutline;
        else loadedTiles = loadedTilesFull;
        int updateRadius = Mathf.FloorToInt(tileUpdateRadiuses[id]);
        for (int x = -updateRadius; x <= updateRadius; x++)
        {
            for (int y = -updateRadius; y <= updateRadius; y++)
            {
                if (tilemaps[id].GetTile<Tile>(location + new Vector3Int(x, y, 0)) != null
                    && !(x == 0 && y == 0))
                {
                    tilemaps[id].SetTile(location + new Vector3Int(x, y, 0),
                        loadedTiles[TilePicker.GetTile(tiles[id], totalBounds, tileVariations, tileUpdateRadiuses, location + new Vector3Int(x, y, 0), id, tileNames)]);
                }
            }
        }
    }

    private void UpdateAllTiles()
    {
        Dictionary<string, Tile> loadedTiles;
        foreach (KeyValuePair<string, Tilemap> entry in tilemaps)
        {
            if (paletteTypes[entry.Key] == PaletteType.Semisolid) loadedTiles = loadedTilesSemisolid;
            else if (paletteTypes[entry.Key] == PaletteType.Danger) loadedTiles = loadedTilesOutline;
            else loadedTiles = loadedTilesFull;

            for (int x = totalBounds.xMin; x <= totalBounds.xMax; x++)
            {
                for (int y = totalBounds.yMin; y <= totalBounds.yMax; y++)
                {
                    if (tilemaps[entry.Key].GetTile<Tile>(new Vector3Int(x, y, 0)) != null
                    && !(x == 0 && y == 0))
                    {
                        tilemaps[entry.Key].SetTile(new Vector3Int(x, y, 0),
                            loadedTiles[TilePicker.GetTile(tiles[entry.Key], totalBounds, tileVariations, tileUpdateRadiuses, new Vector3Int(x, y, 0), entry.Key, tileNames)]);
                    }
                }
            }
        }
    }

    private void DrawTiles()
    {
        Dictionary<string, Tile> loadedTiles;
        foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
        {
            if (paletteTypes[entry.Key] == PaletteType.Semisolid) loadedTiles = loadedTilesSemisolid;
            else if (paletteTypes[entry.Key] == PaletteType.Danger) loadedTiles = loadedTilesOutline;
            else loadedTiles = loadedTilesFull;
            tilemaps[entry.Key].ClearAllTiles();
            foreach (Vector3Int location in entry.Value)
            {
                tilemaps[entry.Key].SetTile(location, loadedTiles[TilePicker.GetTile(tiles[entry.Key], totalBounds, tileVariations, tileUpdateRadiuses, location, entry.Key, tileNames)]);
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

    private void CastIntToPaletteType(Dictionary<string, List<float>> intPalettes)
    {
        //paletteTypes.Clear();
        foreach (KeyValuePair<string, List<float>> entry in intPalettes)
        {
            if (paletteTypes.ContainsKey(entry.Key)) paletteTypes[entry.Key] = (PaletteType)((int)entry.Value[0]);
            else paletteTypes.Add(entry.Key, (PaletteType)((int)entry.Value[0]));
        }
    }

    private Dictionary<string, int> GetPaletteLayerOrder()
    {
        Dictionary<string, int> output = new Dictionary<string, int>();
        foreach (KeyValuePair<string, GameObject> entry in layers)
        {
            output.Add(entry.Key, Mathf.FloorToInt(entry.Value.transform.position.z));
        }
        return output;
    }

    private void SetPaletteLayerOrder(Dictionary<string, List<float>> layerOrder)
    {
        Vector3 pos;
        Vector3 swapPos;
        foreach (KeyValuePair<string, List<float>> entry in layerOrder)
        {
            pos = layers[entry.Key].transform.position;
            foreach (KeyValuePair<string, GameObject> currentLayer in layers)
            {
                if (currentLayer.Value.transform.position.z == (int)entry.Value[0])
                {
                    swapPos = currentLayer.Value.transform.position;
                    currentLayer.Value.transform.position = new Vector3(swapPos.x, swapPos.y, pos.z);
                    break;
                }
            }
            layers[entry.Key].transform.position = new Vector3(pos.x, pos.y, (int)entry.Value[0]);
        }
    }

    private void SetObjectData(Dictionary<string, List<float>> objectData)
    {
        foreach (KeyValuePair<string, List<float>> entry in objectData)
        {
            if (layers.ContainsKey(entry.Key))
            {
                layers[entry.Key].GetComponent<ObjectValueManager>().SetValues(entry.Value);
            }
            else
            {
                GameObject newObject = paletteMenuManager.AddObject(TextUtilities.StripNumbers(entry.Key), entry.Value);
                newObject.GetComponent<ObjectValueManager>().SetValues(entry.Value);
                layers[entry.Key] = newObject;
            }
        }
    }

    public void New()
    {
        LoadFromFile(Application.dataPath + "/Resources/default.lvl");
    }

    public void Load()
    {
        string path = FileUtilities.LoadDialog();
        LoadFromFile(path);
    }

    private void LoadFromFile(string path)
    {
        try
        {
            tiles = LevelParse.ParseFile(path);
            foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
            {
                tilemaps[entry.Key].ClearAllTiles();
            }
            SetObjectData(LevelParse.ParsePaletteInfo(path, "+"));
            CastIntToPaletteType(LevelParse.ParsePaletteInfo(path, ">"));
            SetPaletteLayerOrder(LevelParse.ParsePaletteInfo(path, "!"));
            
            DrawTiles();
            UpdatePalettes();
            playCam.GetComponent<PlayCameraController>().UpdateBounds(CalculateBounds());
        }
        catch (NullReferenceException)
        {
            Debug.Log(path + ": Could not parse level file.");
            return;
        }
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