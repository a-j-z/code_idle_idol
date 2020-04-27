using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class LevelParse : MonoBehaviour
{
    public static Dictionary<Vector3Int, string> ParseFile(string path)
    {
        Dictionary<Vector3Int, string> output = new Dictionary<Vector3Int, string>();

        StreamReader reader;
        try
        {
            reader = new StreamReader(path, Encoding.Default);
        }
        catch (Exception)
        {
            Debug.Log(path + ": Path invalid.");
            throw new NullReferenceException();
        }

        string line, id;
        int x, y;

        int lineNumber = 1;
        using (reader)
        {
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    string[] words = line.Split(' ');
                    if (lineNumber == 1)
                    {
                        if (words[0] != "#validate")
                        {
                            Debug.Log(path + ": file format invalid.");
                            throw new NullReferenceException();
                        }
                    }
                    else
                    {
                        if (words.Length != 3)
                        {
                            Debug.Log(path + ": Line " + lineNumber + ": format invalid.");
                            throw new NullReferenceException();
                        }
                        try
                        {
                            //Assign coordinate values
                            x = Int32.Parse(words[1]);
                            y = Int32.Parse(words[2]);
                        }
                        catch (FormatException)
                        {
                            Debug.Log(path + ": Line " + lineNumber + ": unable to convert value to int.");
                            throw new NullReferenceException();
                        }
                        //Assign ID value
                        id = words[0];

                        //Add to dictionary
                        output.Add(new Vector3Int(x, y, 0), id);
                    }
                }
                lineNumber++;
            }
            while (line != null);
        }
        reader.Close();

        return output;
    }

    public static void EncodeFile(Dictionary<Vector3Int, string> tiles, string path)
    {
        string output = "#validate";

        foreach (KeyValuePair<Vector3Int, string> entry in tiles)
        {
            output += "\n";
            output += entry.Value + " ";
            output += entry.Key.x + " " + entry.Key.y;
        }

        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(output);
        writer.Close();
    }
}

/*
 TODO:
 Handle repeated tiles, throw exception
     */