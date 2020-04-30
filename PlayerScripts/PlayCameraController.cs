using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCameraController : MonoBehaviour
{
    public GameObject player;
    //public float lookAheadX;
    //public float lookAheadY;
    public float smooth;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position + Vector3.back * 10f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 destination;
        destination = (Vector3)player.GetComponent<Rigidbody2D>().position + Vector3.back * 10f; /* +
                Vector3.right * lookAheadX * player.GetComponent<Character>().getXVel() +
            Vector3.up * lookAheadY * player.GetComponent<Character>().getYVel(); */

        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smooth);
    }
}
