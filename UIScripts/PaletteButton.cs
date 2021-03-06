﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PaletteButton : BaseButton
{
    private GameObject paletteTypeButton;

    public void SetPaletteValues(Color[] typeColors, LevelDraw draw)
    {
        paletteTypeButton = transform.GetChild(4).gameObject;
        paletteTypeButton.GetComponent<PaletteTypeButton>().SetValues(GetName(), typeColors, draw);
    }
}
