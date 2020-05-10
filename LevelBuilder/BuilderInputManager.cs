using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Action
{
    [SerializeField] public KeyCode[] input;
    [SerializeField] public bool useInBuild;
    [SerializeField] public bool useInPlay;
    [SerializeField] public bool useInPaletteMenu;
    [SerializeField] public UnityEvent evt;
}

public class BuilderInputManager : MonoBehaviour
{
    public PaletteMenuManager paletteMenuManager;

    [SerializeField] private Action[] actions = new Action[0];

    void Update()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            bool keysPressed = true;
            if (actions[i].input.Length > 1)
            {
                for (int j = 0; j < actions[i].input.Length; j++)
                {
                    if (!Input.GetKey(actions[i].input[j])) keysPressed = false;
                }
            }
            if (Input.GetKeyDown(actions[i].input[actions[i].input.Length - 1]) && keysPressed)
            {
                if ((actions[i].useInBuild && !PlayManager.GetIsPlay()) ||
                    (actions[i].useInPlay && PlayManager.GetIsPlay()))
                {
                    if ((actions[i].useInPaletteMenu && paletteMenuManager.IsPaletteMenuActive()) ||
                        !paletteMenuManager.IsPaletteMenuActive())
                    {
                        actions[i].evt.Invoke();
                    }
                }
            }
        }
    }
}
