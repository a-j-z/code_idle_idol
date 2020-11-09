using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ButtonType : int
{
    Tilemap = 1,
    Player = 2,
    Idol = 3,
    Gates = 4,
    BasicEnemy = 5
}

public class PaletteMenuManager : MonoBehaviour
{
    public GameObject paletteMenu;
    public GameObject paletteMenuContent;

    public GameObject paletteButton;
    public GameObject playerButton;

    public RectTransform moveLayerPreview;
    public RectTransform paletteSelect;
    public LevelDraw draw;
    public Color[] typeColors;

    private List<GameObject> buttons;
    private string[] tileTypes;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        paletteMenu.SetActive(false);
        tileTypes = LevelParse.GetTileTypes();
        buttons = new List<GameObject>();
        RectTransform rt;

        GameObject currentPlayerButton = Instantiate(playerButton, paletteMenuContent.transform);
        currentPlayerButton.GetComponent<PlayerButton>().SetValues(
            "Player", Resources.Load<Sprite>("Tiles/" + tileTypes[0] + "/_" + tileTypes[0] + "_1"), 
            moveLayerPreview, 0, draw);
        currentPlayerButton.GetComponent<PlayerButton>().SetPlayerValues();
        buttons.Add(currentPlayerButton);
        for (int i = 0; i < tileTypes.Length; i++)
        {
            GameObject currentButton = Instantiate(paletteButton, paletteMenuContent.transform);
            currentButton.GetComponent<PaletteButton>().SetValues(
                tileTypes[i], Resources.Load<Sprite>("Tiles/" + tileTypes[i] + "/_" + tileTypes[i] + "_1"), 
                moveLayerPreview, i, draw);
            currentButton.GetComponent<PaletteButton>().SetPaletteValues(typeColors, draw);

            rt = currentButton.gameObject.GetComponent<RectTransform>();
            Vector3 destination = paletteButton.GetComponent<RectTransform>().position +
                new Vector3(paletteMenuContent.GetComponent<RectTransform>().rect.width / 2, 0, 0) +
                paletteMenuContent.GetComponent<RectTransform>().position +
                new Vector3(0, draw.GetLayer(tileTypes[i]) * -80f, 0);

            rt.position = destination;

            buttons.Add(currentButton);
        }
        paletteSelect.transform.SetAsLastSibling();
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject button in buttons)
        {
            Vector3 offset = Vector3.zero;
            if (button.GetComponent<PaletteButton>() != null)
            {
                offset = new Vector3(0, draw.GetLayer(button.GetComponent<PaletteButton>().GetName()) * -80f - 10f, 0);
            }
            else if (button.GetComponent<PlayerButton>() != null)
            {
                offset = new Vector3(0, -10, 0);
            }
            Vector3 destination = new Vector3(paletteMenuContent.GetComponent<RectTransform>().rect.width / 2, 0, 0) +
                paletteMenuContent.GetComponent<RectTransform>().position + offset;
            if (!button.gameObject.transform.Find("PaletteDrag").gameObject.GetComponent<PaletteDrag>().IsDragged())
            {
                if (Vector3.Distance(button.GetComponent<RectTransform>().position, destination) > 0.1f)
                {
                    button.GetComponent<RectTransform>().position =
                    Vector3.SmoothDamp(button.GetComponent<RectTransform>().position, destination, ref velocity, 0.01f);
                }
                else
                {
                    button.GetComponent<RectTransform>().position = destination;
                }
            }
            else
            {
                button.transform.SetAsLastSibling();
                paletteSelect.transform.SetAsLastSibling();
            }
            if (button.GetComponent<PaletteButton>() != null)
            {
                if (button.GetComponent<PaletteButton>().GetName().Equals(tileTypes[draw.GetPalette()]))
                {
                    paletteSelect.position = button.GetComponent<RectTransform>().position;
                }
            }
            i++;
        }     
    }

    public void PaletteMenuSwitch()
    {
        paletteMenu.SetActive(!paletteMenu.activeSelf);
    }

    public bool IsPaletteMenuActive()
    {
        return paletteMenu.activeSelf;
    }
}
