using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteController : MonoBehaviour
{
    public LevelDraw draw;

    private Text text;
    private string[] tileTypes;

    void Start()
    {
        text = GetComponent<Text>();
        tileTypes = LevelParse.GetTileTypes();
    }

    void Update()
    {
        text.text = tileTypes[draw.GetPalette()];

    }
}
