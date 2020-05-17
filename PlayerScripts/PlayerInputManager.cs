using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GameInput
{
    [SerializeField] public KeyCode[] input;
    [SerializeField] public string inputId;
}

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private KeyCode[] kbAxisLeft;
    [SerializeField] private KeyCode[] kbAxisRight;
    [SerializeField] private GameInput[] inputs = new GameInput[0];
    private Dictionary<string, KeyCode[]> inputDict = new Dictionary<string, KeyCode[]>();

    void Start()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            inputDict.Add(inputs[i].inputId, inputs[i].input);
        }
    }

    public bool GetInput(string inputId, string inputType = "-")
    {
        KeyCode[] inputArray = inputDict[inputId];
        for (int i = 0; i < inputArray.Length; i++)
        {
            if (Input.GetKey(inputArray[i]) && inputType.Equals("-")) return true;
            else if (Input.GetKeyDown(inputArray[i]) && inputType.Equals("down")) return true;
            else if (Input.GetKeyUp(inputArray[i]) && inputType.Equals("up")) return true;
        }
        return false;
    }
}
