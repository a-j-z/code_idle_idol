using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GateController : MonoBehaviour
{
    public IdolController idol;
    public PlayManager playManager;
    public float gateThiccness;
    public float offset;
    public LayerMask gateLayer;
    public Sprite theOnlySpriteForNow;

    private Tile theOnlyTileForNow;
    private Dictionary<Vector3, GameObject> gates;
    private Dictionary<Vector3, Vector2> gateSizes;
    private bool isInit = false;

    void Start()
    {
        if (!isInit) { Init(); isInit = true; }
    }

    void FixedUpdate()
    {
        foreach(KeyValuePair<Vector3, GameObject> gate in gates)
        {
            gate.Value.SetActive(!idol.GetIsCarried());
        }
    }

    private void Init()
    {
        gates = new Dictionary<Vector3, GameObject>();
        gates.Add(Vector3.up, new GameObject("GateUp"));
        gates.Add(Vector3.down, new GameObject("GateDown"));
        gates.Add(Vector3.left, new GameObject("GateLeft"));
        gates.Add(Vector3.right, new GameObject("GateRight"));

        gateSizes = new Dictionary<Vector3, Vector2>();
        gateSizes.Add(Vector3.up, new Vector2(1f, gateThiccness)); 
        gateSizes.Add(Vector3.down, new Vector2(1f, gateThiccness)); 
        gateSizes.Add(Vector3.left, new Vector2(gateThiccness, 1f)); 
        gateSizes.Add(Vector3.right, new Vector2(gateThiccness, 1f)); 

        int i = 0;
        foreach (KeyValuePair<Vector3, GameObject> gate in gates)
        {
            gate.Value.layer = LayerUtilities.LayerNumber(gateLayer);
            gate.Value.transform.parent = transform;
            BoxCollider2D collider = gate.Value.AddComponent<BoxCollider2D>();
            collider.size = gateSizes[gate.Key];
            gate.Value.AddComponent<Tilemap>();
            gate.Value.AddComponent<TilemapRenderer>();
            gate.Value.GetComponent<Tilemap>().tileAnchor = new Vector3(0,0,0);
            i++;
        }
    }

    public void UpdatePosition(Vector3 direction, Vector3 position)
    {
        if (!isInit) { Init(); isInit = true; }
        gates[direction].transform.position = new Vector3(position.x, position.y, 0);
        if (direction.Equals(Vector3.up)) gates[direction].transform.position += Vector3.down * offset;
        else if (direction.Equals(Vector3.down)) gates[direction].transform.position += Vector3.up * offset;
        else if (direction.Equals(Vector3.left)) gates[direction].transform.position += Vector3.right * offset;
        else if (direction.Equals(Vector3.right)) gates[direction].transform.position += Vector3.left * offset;
    }

    public void UpdateLength(Vector3 direction, Vector3 length)
    {
        if (!isInit) { Init(); isInit = true; }
        if (gateSizes[direction].x == 1f)
        {
            gates[direction].GetComponent<BoxCollider2D>().size = gateSizes[direction] * new Vector2(length.x, 1);
            gates[direction].GetComponent<BoxCollider2D>().offset = new Vector2((length.x - 1) * 0.5f, 0);
            gates[direction].GetComponent<Tilemap>().ClearAllTiles();
            for (int i = 0; i < length.x; i++)
            {
                theOnlyTileForNow = ScriptableObject.CreateInstance("Tile") as Tile;
                theOnlyTileForNow.sprite = Instantiate(theOnlySpriteForNow);
                gates[direction].GetComponent<Tilemap>().SetTile(new Vector3Int(Mathf.FloorToInt(i), 0, 0), Instantiate(theOnlyTileForNow));
            }
        }
        else if (gateSizes[direction].y == 1f)
        {
            gates[direction].GetComponent<BoxCollider2D>().size = gateSizes[direction] * new Vector2(1, length.y);
            gates[direction].GetComponent<BoxCollider2D>().offset = new Vector2(0, (length.y - 1) * 0.5f);
            gates[direction].GetComponent<Tilemap>().ClearAllTiles();
            for (int i = 0; i < length.y; i++)
            {
                theOnlyTileForNow = ScriptableObject.CreateInstance("Tile") as Tile;
                theOnlyTileForNow.sprite = Instantiate(theOnlySpriteForNow);
                gates[direction].GetComponent<Tilemap>().SetTile(new Vector3Int(0, Mathf.FloorToInt(i), 0), Instantiate(theOnlyTileForNow));
            }
        }
    }
}
