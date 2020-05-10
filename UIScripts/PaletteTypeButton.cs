using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PaletteTypeButton : MonoBehaviour
{
    private Color[] typeColors;
    private string palette;
    private UnityAction<string> action;
    private LevelDraw draw;
    private GameObject typeText;

    private RectTransform rt;
    private Image im;

    public void SetValues(string palette, Color[] typeColors, LevelDraw draw)
    {
        rt = GetComponent<RectTransform>();
        im = GetComponent<Image>();
        this.typeColors = typeColors;

        typeText = gameObject.transform.GetChild(0).gameObject;
        typeText.GetComponent<PaletteTypeText>().SetValues(palette, draw);

        this.draw = draw;
        this.palette = palette;

        action += draw.UpdatePalettes;
    }

    void Update()
    {
        if (MouseUtilities.TouchingUI(rt) && Input.GetMouseButtonDown(0))
        {
            action.Invoke(palette);
        }
        im.color = typeColors[draw.GetPalette(palette) - 1];
    }
}
