using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Action
{
    [SerializeField] public KeyCode input;
    [SerializeField] public bool useInBuild;
    [SerializeField] public bool useInPlay;
    [SerializeField] public UnityEvent evt;
}

public class BuilderInputManager : MonoBehaviour
{
    [SerializeField] private Action[] actions;

    void Update()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            if (Input.GetKeyDown(actions[i].input))
            {
                if ((actions[i].useInBuild && !PlayManager.GetIsPlay()) ||
                    (actions[i].useInPlay && PlayManager.GetIsPlay()))
                {
                    actions[i].evt.Invoke();
                }
            }
        }
    }
}
