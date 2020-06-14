using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    public BoxCollider2D player;
    public PlayManager playManager;
    
    private Dictionary<Vector3, GameObject> exits;
    private bool isInit = false;

    void Start()
    {
        if (!isInit) { Init(); isInit = true; }
    }

    void FixedUpdate()
    {
        foreach(KeyValuePair<Vector3, GameObject> exit in exits)
        {
            if (exit.Value.gameObject.GetComponent<BoxCollider2D>().IsTouching(player))
            {
                playManager.Play(exit.Key * -1);
                break;
            }
        }
    }

    private void Init()
    {
        exits = new Dictionary<Vector3, GameObject>();
        exits.Add(Vector3.up, new GameObject("ExitUp"));
        exits.Add(Vector3.down, new GameObject("ExitDown"));
        exits.Add(Vector3.left, new GameObject("ExitLeft"));
        exits.Add(Vector3.right, new GameObject("ExitRight"));

        foreach (KeyValuePair<Vector3, GameObject> exit in exits)
        {
            exit.Value.transform.parent = transform;
            BoxCollider2D collider = exit.Value.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }

    public void UpdatePosition(Vector3 direction, Vector3 position)
    {
        if (!isInit) { Init(); isInit = true; }
        exits[direction].SetActive(true);
        exits[direction].transform.position = position;
    }

    public void UpdateScale(Vector3 direction, Vector3 scale)
    {
        if (!isInit) { Init(); isInit = true; }
        exits[direction].SetActive(true);
        exits[direction].GetComponent<BoxCollider2D>().size = (Vector2)scale;
    }
}
