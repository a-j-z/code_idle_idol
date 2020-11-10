using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] objects;

    private Dictionary<string, GameObject> objectDict;

    void Start()
    {
        objectDict = new Dictionary<string, GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            objectDict[objects[i].name] = objects[i];
        }
    }

    public GameObject GetObject(string objectName)
    {
        return objectDict[objectName];
    }
}
