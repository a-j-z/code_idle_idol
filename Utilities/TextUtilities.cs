using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUtilities : MonoBehaviour
{
    public static string UnderscoresToSpaces(string word)
    {
        string output = "";
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == '_') output += ' ';
            else output += word[i];
        }
        return output;
    }
}
