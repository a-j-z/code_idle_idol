using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectValueManager : MonoBehaviour
{
    [SerializeField] public UnityEvent<float>[] setEvts;
    [SerializeField] public UnityEvent[] getEvts;

    public void SetValues(List<float> values)
    {
        for (int i = 0; i < Mathf.Min(setEvts.Length, values.Count); i++)
        {
            setEvts[i].Invoke(values[i]);
        }
    }

    public float[] GetValues()
    {
        float[] output = new float[getEvts.Length];
        for (int i = 0; i < getEvts.Length; i++)
        {
            output[i] = 0f;
        }
        return output;
    }
}
