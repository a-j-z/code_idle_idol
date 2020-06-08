using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Sprite spawnSymbol;

    private Dictionary<Vector3, GameObject> spawns;

    void Start()
    {
        spawns = new Dictionary<Vector3, GameObject>();
        spawns.Add(Vector3.up, new GameObject("SpawnUp"));
        spawns.Add(Vector3.down, new GameObject("SpawnDown"));
        spawns.Add(Vector3.left, new GameObject("SpawnLeft"));
        spawns.Add(Vector3.right, new GameObject("SpawnRight"));

        foreach (KeyValuePair<Vector3, GameObject> spawn in spawns)
        {
            spawn.Value.transform.parent = transform;
            SpriteRenderer sr = spawn.Value.AddComponent<SpriteRenderer>();
            sr.sprite = spawnSymbol;
        }
    }

    public void UpdatePosition(Vector3 direction, Vector3 position)
    {
        if (position.Equals(Vector3.zero))
        {
            spawns[direction].SetActive(false);
        }
        else
        {
            spawns[direction].SetActive(true);
            spawns[direction].transform.position = position;
        }
        
    }

    public GameObject GetSpawn(Vector3 direction)
    {
        return spawns[direction];
    }
}
