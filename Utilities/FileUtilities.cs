using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

public class FileUtilities : MonoBehaviour
{
    public static string SaveDialog()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save Level", "", "level.lvl", "lvl");
        return path;
    }

    public static string LoadDialog()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Load Level", "", "lvl", false);
        if (path.Length > 0) return path[0];
        else return "";
    }
}
