using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongLine : MonoBehaviour
{
    public NodeLineDraw path;
    public float speed;

    private bool isPlayBuffer;
    private bool movingForward;

    void Start()
    {
        isPlayBuffer = false;
    }

    void Update()
    {
        if (PlayManager.GetIsPlay() && !isPlayBuffer) 
        {
            transform.position = path.GetNodeLocation(0);
            movingForward = true;
        }

        if (!PlayManager.GetIsPlay())
        {
            transform.position = (path.GetNodeLocation(0) + path.GetNodeLocation(1)) / 2f;
        }
        else
        {
            Vector3 posChange;
            if (movingForward) posChange = Vector3.Normalize(path.GetNodeLocation(1) - path.GetNodeLocation(0)) * speed * Time.deltaTime;
            else  posChange = Vector3.Normalize(path.GetNodeLocation(0) - path.GetNodeLocation(1)) * speed * Time.deltaTime;
            transform.position += posChange;
            if (posChange.x > 0) transform.localScale = new Vector3(1,1,1);
            else if (posChange.x < 0) transform.localScale = new Vector3(-1,1,1);
            int index = movingForward ? 1 : 0;
            if (Vector2.Distance((Vector2)transform.position, (Vector2)path.GetNodeLocation(index)) <= speed * Time.deltaTime)
            {
                movingForward = !movingForward;
            }
        }
        isPlayBuffer = PlayManager.GetIsPlay();
    }
}
