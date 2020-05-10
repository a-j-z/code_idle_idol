using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaletteMenuManager : MonoBehaviour
{
    public GameObject paletteMenu;
    public GameObject paletteMenuContent;
    public GameObject paletteButton;
    public RectTransform moveLayerPreview;
    public RectTransform paletteSelect;
    public LevelDraw draw;
    public Color[] typeColors;

    private List<GameObject> paletteButtons;
    private string[] tileTypes;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        paletteMenu.SetActive(false);
        tileTypes = LevelParse.GetTileTypes();
        paletteButtons = new List<GameObject>();
        RectTransform rt;

        for (int i = 0; i < tileTypes.Length; i++)
        {
            GameObject currentButton = Instantiate(paletteButton, paletteMenuContent.transform);
            currentButton.GetComponent<PaletteButton>().SetValues(
                tileTypes[i], typeColors, Resources.Load<Sprite>("Tiles/" + tileTypes[i] + "/_" + tileTypes[i] + "_1"), moveLayerPreview, i, draw);

            rt = currentButton.gameObject.GetComponent<RectTransform>();
            Vector3 destination = paletteButton.GetComponent<RectTransform>().position +
                new Vector3(paletteMenuContent.GetComponent<RectTransform>().rect.width / 2, 0, 0) +
                paletteMenuContent.GetComponent<RectTransform>().position +
                new Vector3(0, draw.GetLayer(tileTypes[i]) * -80f, 0);

            rt.position = destination;

            paletteButtons.Add(currentButton);
        }
        paletteSelect.transform.SetAsLastSibling();
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject button in paletteButtons)
        {
            Vector3 destination = paletteButton.GetComponent<RectTransform>().position +
                new Vector3(paletteMenuContent.GetComponent<RectTransform>().rect.width / 2, 0, 0) +
                paletteMenuContent.GetComponent<RectTransform>().position +
                new Vector3(0, draw.GetLayer(button.GetComponent<PaletteButton>().GetName()) * -80f, 0);
            if (!button.gameObject.transform.GetChild(4).gameObject.GetComponent<PaletteDrag>().IsDragged())
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
            if (button.GetComponent<PaletteButton>().GetName().Equals(tileTypes[draw.GetPalette()]))
            {
                paletteSelect.position = button.GetComponent<RectTransform>().position;
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
