using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PaletteButton : MonoBehaviour
{
    private RectTransform rt;
    private Text m_Name;
    private Image m_Preview;
    private UnityAction<int> action;
    private int index;

    private GameObject paletteTypeButton;
    private GameObject paletteDrag;

    public void SetValues(string name, Color[] typeColors, Sprite preview, RectTransform moveLayerPreview, int index, LevelDraw draw)
    {
        rt = GetComponent<RectTransform>();
        m_Name = gameObject.transform.GetChild(0).GetComponent<Text>();
        m_Preview = gameObject.transform.GetChild(2).GetComponent<Image>();
        
        paletteTypeButton = gameObject.transform.GetChild(3).gameObject;
        paletteTypeButton.GetComponent<PaletteTypeButton>().SetValues(name, typeColors, draw);

        paletteDrag = gameObject.transform.GetChild(4).gameObject;
        paletteDrag.GetComponent<PaletteDrag>().SetValues(name, moveLayerPreview, draw);

        m_Name.text = name;
        m_Preview.sprite = preview;
        this.index = index;
        action += draw.SetPalette;
    }

    void Update()
    {
        if (MouseUtilities.TouchingUI(rt) && Input.GetMouseButtonDown(0))
        {
            action.Invoke(index);
        }
    }

    public string GetName()
    {
        return m_Name.text;
    }
}
