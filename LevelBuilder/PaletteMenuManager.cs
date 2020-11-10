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
    public Camera cam;

    public GameObject paletteButton;
    public GameObject playerButton;
    public GameObject basicEnemyButton;

    public RectTransform moveLayerPreview;
    public RectTransform paletteSelect;
    public LevelDraw draw;
    public GameObject player;
    public ObjectManager objectManager;
    public Color[] typeColors;

    private List<GameObject> buttons;
    private string[] tileTypes;
    private Vector3 velocity = Vector3.zero;

    private Dictionary<string, int> objectCount;

    void Start()
    {
        objectCount = new Dictionary<string, int>();
        paletteMenu.SetActive(false);
        tileTypes = LevelParse.GetTileTypes();
        buttons = new List<GameObject>();
        RectTransform rt;

        GameObject currentPlayerButton = Instantiate(playerButton, paletteMenuContent.transform);
        currentPlayerButton.GetComponent<PlayerButton>().SetValues(
            "Player", player.GetComponent<SpriteRenderer>().sprite, 
            moveLayerPreview, "Player", draw);
        currentPlayerButton.GetComponent<PlayerButton>().SetPlayerValues(
            player.GetComponent<PlayerMovement>().speed, player.GetComponent<PlayerController>().jumpHeight, player);
        buttons.Add(currentPlayerButton);
        for (int i = 0; i < tileTypes.Length; i++)
        {
            GameObject currentButton = Instantiate(paletteButton, paletteMenuContent.transform);
            currentButton.GetComponent<PaletteButton>().SetValues(
                tileTypes[i], Resources.Load<Sprite>("Tiles/" + tileTypes[i] + "/_" + tileTypes[i] + "_1"), 
                moveLayerPreview, tileTypes[i], draw);
            currentButton.GetComponent<PaletteButton>().SetPaletteValues(typeColors, draw);

            rt = currentButton.gameObject.GetComponent<RectTransform>();
            Vector3 destination = paletteButton.GetComponent<RectTransform>().position +
                new Vector3(paletteMenuContent.GetComponent<RectTransform>().rect.width / 2, 0, 0) +
                paletteMenuContent.GetComponent<RectTransform>().position +
                new Vector3(0, draw.GetLayerIndex(tileTypes[i]) * -80f, 0);

            rt.position = destination;
            buttons.Add(currentButton);
        }
        paletteSelect.transform.SetAsFirstSibling();
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject button in buttons)
        {
            Vector3 offset = new Vector3(0, draw.GetLayerIndex(button.GetComponent<BaseButton>().GetName()) * -80f - 10f, 0);

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
                paletteSelect.transform.SetAsFirstSibling();
            }
            if (button.GetComponent<BaseButton>().GetName().Equals(draw.GetLayer()))
            {
                paletteSelect.position = button.GetComponent<RectTransform>().position;
            }
            i++;
        }     
    }

    public GameObject AddObject(string objectName, List<float> data)
    {
        if (objectCount.ContainsKey(objectName)) objectCount[objectName]++;
        else objectCount[objectName] = 1;
        
        GameObject newObject = Instantiate(objectManager.GetObject(objectName));
        newObject.GetComponent<NodeLineDraw>().cam = cam;
        newObject.GetComponent<NodeLineDraw>().draw = draw;
        newObject.name = objectName + objectCount[objectName];
        draw.AddLayer(newObject.name, newObject);

        GameObject newObjectButton = Instantiate(basicEnemyButton, paletteMenuContent.transform);
        newObjectButton.GetComponent<BaseButton>().SetValues(
            newObject.name, newObject.GetComponentInChildren<SpriteRenderer>().sprite, 
            moveLayerPreview, newObject.name, draw);
        newObjectButton.GetComponent<BasicEnemyButton>().SetBasicEnemyValues(
            data[0], newObject
        );
        buttons.Add(newObjectButton);
        return newObject;
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
