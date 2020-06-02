using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class LevelParse : MonoBehaviour
{
    public static Dictionary<string, List<Vector3Int>> ParseFile(string path)
    {
        Dictionary<string, List<Vector3Int>> output = new Dictionary<string, List<Vector3Int>>();

        string[] tileTypes = GetTileTypes();
        for (int i = 0; i < tileTypes.Length; i++)
        {
            output.Add(tileTypes[i], new List<Vector3Int>());
        }

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
                    else if (words[0] != ">" && words[0] != "!")
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
                        output[id].Add(new Vector3Int(x, y, 0));
                    }
                }
                lineNumber++;
            }
            while (line != null);
        }
        reader.Close();

        return output;
    }

    // > = palette type
    // ! = palette layer order
    public static Dictionary<string, int> ParsePaletteInfo(string path, string identifier)
    {
        Dictionary<string, int> output = new Dictionary<string, int>();

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

        string line;
        int paletteType;

        int lineNumber = 1;
        using (reader)
        {
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    string[] words = line.Split(' ');
                    if (words[0] == identifier)
                    {
                        try
                        {
                            paletteType = Int32.Parse(words[2]);
                        }
                        catch (FormatException)
                        {
                            Debug.Log(path + ": Line " + lineNumber + ": unable to convert value " + words[2] + " to int.");
                            throw new NullReferenceException();
                        }
                        output.Add(words[1], paletteType);
                    }
                }
                lineNumber++;
            }
            while (line != null);
        }
        reader.Close();

        return output;
    }

    public static void EncodeFile(Dictionary<string, List<Vector3Int>> tiles, Dictionary<string, int> paletteTypes, Dictionary<string, int> layerOrder, string path)
    {
        string output = "#validate";

        foreach (KeyValuePair<string, List<Vector3Int>> entry in tiles)
        {
            foreach (Vector3Int location in entry.Value)
            {
                output += "\n";
                output += entry.Key + " ";
                output += location.x + " " + location.y;
            }
        }

        foreach (KeyValuePair<string, int> entry in paletteTypes)
        {
            output += "\n> ";
            output += entry.Key + " " + entry.Value;
        }

        foreach (KeyValuePair<string, int> entry in layerOrder)
        {
            output += "\n! ";
            output += entry.Key + " " + entry.Value;
        }

        try
        {
            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(output);
            writer.Close();
        }
        catch (ArgumentException)
        {
            Debug.Log("could not write in this file.");
            throw new ArgumentException();
        }
    }

    public static string[] GetTileTypes()
    {
        string[] tileTypes = Directory.GetDirectories(Application.dataPath + "/Resources/Tiles");

        string[] splitTileType;
        string tileType;
        char[] splitter = { '/', '\\' };

        for (int i = 0; i < tileTypes.Length; i++)
        {
            splitTileType = tileTypes[i].Split(splitter);
            tileType = splitTileType[splitTileType.Length - 1];
            tileTypes[i] = tileType;
        }

        return tileTypes;
    }

    public static Dictionary<string, float> LoadTileData(string filename)
    {
        Dictionary<string, float> output = new Dictionary<string, float>();

        StreamReader reader;
        try
        {
            reader = new StreamReader(Application.dataPath + "/Resources/Tiles/" + filename + ".txt", Encoding.Default);
        }
        catch (Exception)
        {
            Debug.Log("Could not open sizes.txt.");
            throw new NullReferenceException();
        }

        string line, id;
        float size;

        int lineNumber = 1;
        using (reader)
        {
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    string[] words = line.Split(' ');
                    if (words.Length != 2)
                    {
                        Debug.Log("sizes.txt: Line " + lineNumber + ": format invalid.");
                        throw new NullReferenceException();
                    }
                    try
                    {
                        size = float.Parse(words[1]);
                    }
                    catch (FormatException)
                    {
                        Debug.Log("sizes.txt: Line " + lineNumber + ": unable to convert value to int.");
                        throw new NullReferenceException();
                    }
                    id = words[0];
                    output.Add(id, size);
                }
                lineNumber++;
            }
            while (line != null);
        }
        reader.Close();

        return output;
    }

    public static Dictionary<string, int> GetTileVariations()
    {
        Dictionary<string, int> output = new Dictionary<string, int>();

        string[] tileTypes = GetTileTypes();
        string[] splitFileName;
        string fileName;
        char[] splitter = { '/', '\\' };
        
        for (int i = 0; i < tileTypes.Length; i++)
        {
            string[] fileArray = Directory.GetFiles(Application.dataPath + "/Resources/Tiles/" + tileTypes[i], "*.png", SearchOption.AllDirectories);
            for (int j = 0; j < fileArray.Length; j++)
            {
                splitFileName = fileArray[j].Split(splitter);
                fileName = splitFileName[splitFileName.Length - 1];
                splitFileName = fileName.Split('_');
                fileName = splitFileName[0];
                for (int word = 1; word < splitFileName.Length-1; word++)
                {
                    fileName += "_" + splitFileName[word];
                }
                Debug.Log(fileName);
                if (output.ContainsKey(fileName))
                {
                    output[fileName]++;
                }
                else
                {
                    output.Add(fileName, 1);
                }
            }
        }

        return output;
    }
}