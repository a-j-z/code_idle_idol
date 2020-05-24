using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteTypeText : MonoBehaviour
{
    private string palette;
    private LevelDraw draw;

    private Text text;


    public void SetValues(string palette, LevelDraw draw)
    {
        text = GetComponent<Text>();

        this.palette = palette;
        this.draw = draw;
    }

    void Update()
    {
        switch (draw.GetPalette(palette))
        {
            case 1:
                text.text = "Collidable";
                break;
            case 2:
                text.text = "Noncollidable";
                break;
            case 3:
                text.text = "Semisolid";
                break;
            case 4:
                text.text = "Idol Filter";
                break;
            default:
                text.text = "";
                break;
        }
    }
}
