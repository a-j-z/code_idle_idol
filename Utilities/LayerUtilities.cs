using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUtilities : MonoBehaviour
{
    public static int LayerNumber(LayerMask layer)
    {
        return Mathf.RoundToInt(Mathf.Log(layer.value, 2));
    }

    public static int LayerNumbersToMask(int[] layers)
    {
        int output = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            output = output | 1 << layers[i];
        }
        return output;
    }
}
